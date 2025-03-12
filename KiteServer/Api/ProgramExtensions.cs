using Api.Middleware;
using Application.Services;
using Application.Validations;
using Domain.Aggregate;
using EasyCaching.Core.Configurations;
using EasyCaching.Serialization.SystemTextJson.Configurations;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Refit;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Infrastructure.Services;
using Mapster;
using MapsterMapper;
using Repository.Repositories;
using Repository.Services;

namespace Api;

/// <summary>
/// 服务注册扩展类
/// </summary>
[ApiExplorerSettings(IgnoreApi = true)]
public static class ProgramExtensions
{
    /// <summary>
    /// 添加数据库服务
    /// </summary>
    public static void AddCustomDatabase(this WebApplicationBuilder builder)
    {
        SnowFlakeSingle.WorkId = 3;
        var configuration = builder.Configuration;
        
        // 获取数据库类型和连接字符串
        var dbType = configuration["Database:Type"] ?? "PostgreSQL";
        var connectionStrings = configuration.GetSection("Database:ConnectionStrings");
        var connectionString = connectionStrings[dbType];
        
        // 根据配置选择数据库类型（使用.NET 8的switch表达式）
        var sugarDbType = dbType switch
        {
            "MySQL" => SqlSugar.DbType.MySql,
            "SqlServer" => SqlSugar.DbType.SqlServer,
            "Oracle" => SqlSugar.DbType.Oracle,
            "Sqlite" => SqlSugar.DbType.Sqlite,
            "PostgreSQL" or _ => SqlSugar.DbType.PostgreSQL
        };
        
        SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = sugarDbType,
                ConnectionString = connectionString,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            },
            db =>
            {
                db.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //支持实体自定义属性：[Key]
                    EntityService = (property, column) =>
                    {
                        var attributes = property.GetCustomAttributes(true);
                        if (attributes.Any(it => it is KeyAttribute))
                        {
                            column.IsPrimarykey = true;
                        }
                    },
                    EntityNameService = (type, entity) =>
                    {
                        var attributes = type.GetCustomAttributes(true);
                        if (attributes.Any(it => it is TableAttribute))
                        {
                            entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute)
                                .Name;
                        }
                    }
                };
                //查询过滤器
                db.QueryFilter.AddTableFilter<IDeleted>(x => x.IsDeleted == false);
                db.Aop.DataExecuting = (oldValue, entityInfo) =>
                {
                    switch (entityInfo.OperationType)
                    {
                        case DataFilterType.InsertByObject:
                            if (entityInfo.PropertyName == "CreateTime")
                            {
                                entityInfo.SetValue(DateTime.Now);
                            }

                            if (entityInfo.PropertyName == "UpdateTime")
                            {
                                entityInfo.SetValue(DateTime.Now);
                            }

                            if (entityInfo.PropertyName == "IsDeleted")
                            {
                                entityInfo.SetValue(false);
                            }

                            break;
                        case DataFilterType.UpdateByObject:
                            if (entityInfo.PropertyName == "UpdateTime")
                            {
                                entityInfo.SetValue(DateTime.Now);
                            }

                            break;
                        case DataFilterType.DeleteByObject:
                            if (entityInfo.PropertyName == "UpdateTime")
                            {
                                entityInfo.SetValue(DateTime.Now);
                            }

                            if (entityInfo.PropertyName == "IsDeleted")
                            {
                                entityInfo.SetValue(true);
                            }

                            break;
                    }
                };
                if (builder.Environment.IsDevelopment())
                {
                    db.Aop.OnLogExecuting = (sql, pars) =>
                    {
                        Log.Logger.Information(UtilMethods.GetNativeSql(sql, pars));
                    };
                }
            });

        // 初始化表结构
        ISugarUnitOfWork<DBContext> context = new SugarUnitOfWork<DBContext>(sqlSugar);
        builder.Services.AddSingleton(context);
        
        // 注册自动建表服务
        builder.Services.AddSingleton(new AutoGeneratedService(sqlSugar));
    }

    /// <summary>
    /// 添加常规服务
    /// </summary>
    public static void AddCustomService(this WebApplicationBuilder builder)
    {
        builder.Services.AddAutofac();
        builder.Services.AddEndpointsApiExplorer();
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(cbuilder =>
            {
                var controllerBaseType = typeof(ControllerBase);
                cbuilder.RegisterAssemblyTypes(typeof(Program).Assembly)
                    .Where(t => controllerBaseType.IsAssignableFrom(t) && t != controllerBaseType)
                    .PropertiesAutowired();
            });
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new LongToStringConverter());
            options.JsonSerializerOptions.Converters.Add(new TimeOnlyConverter());
            options.JsonSerializerOptions.Converters.Add(new DateOnlyConverter());
            options.JsonSerializerOptions.Converters.Add(new DateTimeConverter());
            options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
            options.JsonSerializerOptions.PropertyNamingPolicy = null;
        });

        //Mapster配置
        Mapster.TypeAdapterConfig.GlobalSettings.Scan(typeof(MapsterConfiguration).Assembly);
        // 注册 Mapster 的 IMapper
        var config = TypeAdapterConfig.GlobalSettings;
        builder.Services.AddSingleton(config);
        builder.Services.AddScoped<IMapper>(sp => new Mapper(sp.GetRequiredService<TypeAdapterConfig>()));

        builder.AddConfiguraMvcSetting(builder.Environment);
        builder.AddHttpContextAccessor();
        builder.AddCorsService();
        builder.AddRedisService();
        builder.AddSwaggerService();
        builder.AddSerilogService();
        builder.AddRefitService();
        builder.AddCustomeAuthentication();
        builder.AddMediatRService();

        // 添加 SFTP 服务
        builder.Services.AddScoped<ISftpService, SftpService>();
    }

    /// <summary>
    /// 添加跨域服务
    /// </summary>
    private static void AddCorsService(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("default", config =>
            {
                if (builder.Environment.IsProduction())
                {
                    var origins = builder.Configuration["CorsOrigins"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.RemovePostFix("/"))
                        .ToArray();
                    config
                        .WithOrigins(origins)
                        .SetIsOriginAllowedToAllowWildcardSubdomains()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders("Content-Range", "Accept-Ranges", "Content-Length", "Content-Type")
                        .AllowCredentials();
                }
                else
                {
                    // 开发/测试环境配置
                    config
                        .AllowAnyMethod()
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Range", "Accept-Ranges", "Content-Length", "Content-Type");
                }
            });
        });
    }

    /// <summary>
    /// 添加swagger服务
    /// </summary>
    private static void AddSwaggerService(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(x =>
        {
            x.SwaggerDoc("v1", new OpenApiInfo { Title = $"WebApiService", Version = "v1" });

            x.CustomOperationIds(apiDesc =>
            {
                var controllerAction = apiDesc.ActionDescriptor as ControllerActionDescriptor;
                return controllerAction.ControllerName + "-" + controllerAction.ActionName;
            });
            x.DocumentFilter<SwaggerEnumFilter>();
            // 添加控制器层注释，true表示显示控制器注释
            x.IncludeXmlComments(@"SwaggerXml/api.xml", true);
            x.IncludeXmlComments(@"SwaggerXml/application.xml", true);
            x.IncludeXmlComments(@"SwaggerXml/domain.xml", true);
            x.IncludeXmlComments(@"SwaggerXml/domainshared.xml", true);
        });
    }

    /// <summary>
    /// 添加MediatR服务
    /// </summary>
    private static void AddMediatRService(this WebApplicationBuilder builder)
    {
        builder.Services.AddMediatR(option =>
        {
            option.RegisterServicesFromAssembly(typeof(Program).Assembly);
            option.RegisterServicesFromAssembly(typeof(MediatorModule).Assembly);
        });
        var serviceProvider = builder.Services.BuildServiceProvider();
        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>(cbuilder =>
            {
                cbuilder.RegisterModule(new MediatorModule());
                cbuilder.RegisterModule(new QueriesModule(serviceProvider));
            });
    }

    /// <summary>
    /// 添加Redis服务
    /// </summary>
    private static void AddRedisService(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        var isEnabled = configuration.GetValue<bool>("Redis:Enabled");

        if (!isEnabled)
        {
            // 当Redis禁用时，注册一个内存缓存提供程序
            builder.Services.AddEasyCaching(options => { options.UseInMemory("default"); });
            return;
        }

        var host = configuration.GetValue<string>("Redis:Host");
        var db = configuration.GetValue<int>("Redis:Db");
        var port = configuration.GetValue<int>("Redis:Port");
        var password = configuration.GetValue<string>("Redis:Password");

        builder.Services.AddEasyCaching(options =>
        {
            options.UseRedis(config =>
                {
                    config.DBConfig.Database = db;
                    config.DBConfig.Endpoints.Add(new ServerEndPoint(host, port));
                    config.DBConfig.Password = password;
                    config.SerializerName = "redis";
                }, "redis")
                .WithSystemTextJson("redis");
        });
    }

    /// <summary>
    /// 添加校验过滤服务
    /// </summary>
    private static void AddConfiguraMvcSetting(this WebApplicationBuilder builder, IWebHostEnvironment env)
    {
        builder.Services.AddMvc(options =>
        {
            options.Filters.Add(typeof(ResultFilter));
            if (env.IsProduction())
            {
                options.Filters.Add(typeof(ValidateModelStateFilter));
                options.Filters.Add(typeof(ExceptionFilter));
            }
            else
            {
                options.Filters.Add(typeof(DevValidateModelStateFilter));
                options.Filters.Add(typeof(DevExceptionFilter));
            }
        });
        builder.Services.AddFluentValidationAutoValidation();
        ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
        ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
        builder.Services.AddValidatorsFromAssemblyContaining<SignInValidator>();
        builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
    }

    /// <summary>
    /// 注入Refit服务
    /// </summary>
    private static void AddRefitService(this WebApplicationBuilder builder)
    {
        builder.Services.AddTransient<HttpClientLoggingHandler>();
        var settings = new RefitSettings
        {
            ContentSerializer = new SystemTextJsonContentSerializer(new JsonSerializerOptions
            {
                Converters = { new DateTimeConverter() },
                //反序列化不区分大小写
                PropertyNameCaseInsensitive = true
            })
        };
        builder.Services.AddRefitClient<IDingTalkService>(settings)
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["DingTalk:CompanyApiUri"]))
            .AddHttpMessageHandler<HttpClientLoggingHandler>();
    }

    /// <summary>
    /// 注入自定义Jwt认证服务
    /// </summary>
    private static void AddCustomeAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                var secretByte = Encoding.UTF8.GetBytes(builder.Configuration["Authentication:SecretKey"]);
                option.TokenValidationParameters = new TokenValidationParameters()
                {
                    //验证发布者
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration["Authentication:Issuer"],
                    //验证接收者
                    ValidateAudience = true,
                    ValidAudience = builder.Configuration["Authentication:Audience"],
                    //验证是否过期
                    ValidateLifetime = true,
                    //验证私钥
                    IssuerSigningKey = new SymmetricSecurityKey(secretByte)
                };
                option.SaveToken = true;
                option.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        context.Response.Clear();
                        context.Response.ContentType = "application/json;charset=utf-8";
                        context.Response.Headers.Append("Access-Control-Allow-Origin", "*");
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        var responseStr =
                            AjaxResponse.Result(false, StatusCodes.Status401Unauthorized, "用户登录已失效或无访问权限!");
                        context.Response.WriteAsJsonAsync(responseStr);
                        return Task.CompletedTask;
                    }
                };
            });
    }

    /// <summary>
    /// 注入Serilog日志服务
    /// </summary>
    private static void AddSerilogService(this WebApplicationBuilder builder)
    {
        // 使用日志
        builder.Host.UseSerilog((context, logger) =>
        {
            var config = context.Configuration;
            logger.ReadFrom.Configuration(config);

            logger.WriteTo.Console();
        });
    }

    /// <summary>
    /// 启用HttpContextAccessor
    /// </summary>
    private static void AddHttpContextAccessor(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddScoped<ICurrentUser, CurrentUser>();
    }

    /// <summary>
    /// 配置中间件
    /// </summary>
    public static void UseCustomMiddleware(this WebApplication app)
    {
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseCors("default");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapControllers();
        
        // 执行自动建表
        var autoGeneratedService = app.Services.GetRequiredService<Repository.Services.AutoGeneratedService>();
        autoGeneratedService.InitTables();
    }
}