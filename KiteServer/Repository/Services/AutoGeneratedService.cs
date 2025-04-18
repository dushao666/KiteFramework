using Domain.Aggregate;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using SqlSugar;

namespace Repository.Services
{
    /// <summary>
    /// 自动建表服务
    /// </summary>
    public class AutoGeneratedService
    {
        private readonly SqlSugarScope _db;
        private readonly IConfiguration _configuration;

        public AutoGeneratedService(SqlSugarScope db, IConfiguration configuration)
        {
            _db = db;
            _configuration = configuration;
        }

        /// <summary>
        /// 自动建表
        /// </summary>
        public void InitTables()
        {
            // 获取所有实现了IAutoGenerated接口的类型
            var types = GetAutoGeneratedTypes();
            if (types.Any())
            {
                // 检查是否需要重新创建数据库
                bool recreateDatabase = _configuration.GetValue<bool>("SeedData:RecreateDatabase", false);
                
                if (recreateDatabase)
                {
                    Console.WriteLine("正在删除所有表...");
                    // 获取所有表名
                    var tableNames = types.Select(t => _db.EntityMaintenance.GetEntityInfo(t).DbTableName).ToArray();
                    
                    // 获取数据库中已存在的表
                    var existingTables = _db.DbMaintenance.GetTableInfoList().Select(t => t.Name).ToArray();
                    
                    // 只删除已存在的表
                    var tablesToDrop = tableNames.Where(t => existingTables.Contains(t)).ToArray();
                    if (tablesToDrop.Any())
                    {
                        _db.DbMaintenance.DropTable(tablesToDrop);
                        Console.WriteLine($"已删除 {tablesToDrop.Length} 个表，准备重新创建...");
                    }
                    else
                    {
                        Console.WriteLine("没有需要删除的表，将直接创建新表...");
                    }
                }
                
                // 配置SqlSugar的EntityProvider，处理可空属性
                _db.CurrentConnectionConfig.ConfigureExternalServices ??= new ConfigureExternalServices();
                var originalEntityService = _db.CurrentConnectionConfig.ConfigureExternalServices.EntityService;
                
                _db.CurrentConnectionConfig.ConfigureExternalServices.EntityService = (property, column) =>
                {
                    // 先执行原来的EntityService处理（如果有的话）
                    originalEntityService?.Invoke(property, column);
                    
                    // 然后处理可空属性
                    if (IsNullableProperty(property))
                    {
                        column.IsNullable = true;
                        Console.WriteLine($"设置属性 {property.DeclaringType?.Name}.{property.Name} 为可空");
                    }
                };
                
                // 使用SqlSugar的CodeFirst功能创建表
                _db.CodeFirst.InitTables(types.ToArray());
                Console.WriteLine("表结构初始化完成");
            }
        }

        /// <summary>
        /// 判断属性是否为可空类型
        /// </summary>
        private bool IsNullableProperty(PropertyInfo property)
        {
            // 如果属性已经有SugarColumn特性并明确设置了IsNullable，直接返回
            var sugarColumnAttr = property.GetCustomAttribute<SugarColumn>();
            if (sugarColumnAttr != null && sugarColumnAttr.IsNullable)
            {
                return true;
            }
            
            // 检查是否为可空值类型（如int?）
            var propertyType = property.PropertyType;
            if (Nullable.GetUnderlyingType(propertyType) != null)
            {
                return true;
            }
            
            // 检查是否为带有?的字符串或引用类型
            // C# 8.0引入的可空引用类型
            bool isNullableReferenceType = false;
            
            // 检查属性是否为引用类型
            if (!propertyType.IsValueType)
            {
                // 获取属性的NullabilityInfo（.NET 6+）
                try
                {
                    // 尝试通过属性名称判断是否有"?"后缀标记（在源代码中的写法）
                    // 这种方法不是100%可靠，但在某些情况下可以帮助识别
                    if (property.GetCustomAttributes()
                        .Any(a => a.GetType().Name.Contains("NullableAttribute") ||
                                a.GetType().Name.Contains("Nullable")))
                    {
                        isNullableReferenceType = true;
                    }
                    
                    // 检查C#编译器生成的NullableAttribute
                    var nullableAttributes = property.CustomAttributes
                        .Where(a => a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableAttribute" || 
                                   a.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")
                        .ToList();
                    
                    if (nullableAttributes.Any())
                    {
                        isNullableReferenceType = true;
                    }
                }
                catch
                {
                    // 忽略异常，继续使用默认值
                }
            }
            
            return isNullableReferenceType;
        }

        /// <summary>
        /// 获取所有实现了IAutoGenerated接口的类型
        /// </summary>
        private List<Type> GetAutoGeneratedTypes()
        {
            var autoGeneratedTypes = new List<Type>();
            
            // 获取所有程序集
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            foreach (var assembly in assemblies)
            {
                try
                {
                    // 获取程序集中所有类型
                    var types = assembly.GetTypes();
                    
                    // 筛选出实现了IAutoGenerated接口的类型
                    var filteredTypes = types.Where(t => 
                        t.IsClass && 
                        !t.IsAbstract && 
                        typeof(IAutoGenerated).IsAssignableFrom(t));
                    
                    autoGeneratedTypes.AddRange(filteredTypes);
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // 处理加载类型时可能出现的异常
                    var loadableTypes = ex.Types.Where(t => t != null);
                    var filteredTypes = loadableTypes.Where(t => 
                        t.IsClass && 
                        !t.IsAbstract && 
                        typeof(IAutoGenerated).IsAssignableFrom(t));
                    
                    autoGeneratedTypes.AddRange(filteredTypes);
                }
                catch
                {
                    // 忽略其他异常
                }
            }
            
            return autoGeneratedTypes;
        }
    }
}
