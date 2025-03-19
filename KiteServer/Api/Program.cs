using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);
builder.LogStartupInformation();
//  Kestrel С
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = 2L * 1024 * 1024 * 1024; // Ϊ 2GB
    serverOptions.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5); // ͷʱʱΪ 5 
});
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024; // 2GB
});

builder.AddCustomDatabase();
builder.AddCustomService();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsEnvironment("test"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseKnife4UI(c =>
    {
        c.RoutePrefix = string.Empty; // serve the UI at root
        c.SwaggerEndpoint("/v1/api-docs", "Service.Api v1");
    });
    app.MapSwagger("{documentName}/api-docs", x => { x.SerializeAsV2 = true; });
}

app.MapControllers();
app.UseCookiePolicy();
app.UseCors("default");
app.UseCustomMiddleware(); // 中间件配置（包括表创建和种子数据初始化）

app.UseAuthentication();
app.UseAuthorization();
// 配置静态文件服务
app.UseStaticFiles();
try
{
    app.Logger.LogInformation("Starting web host ({ApplicationName})...", AppSetting.Name);
    app.Run();

    return 0;
}
catch (Exception ex)
{
    app.Logger.LogCritical(ex, "Host terminated unexpectedly ({ApplicationName})...", AppSetting.Name);
    return 1;
}
finally
{
    Log.CloseAndFlush();
}