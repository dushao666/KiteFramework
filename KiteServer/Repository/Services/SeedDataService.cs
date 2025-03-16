using Microsoft.Extensions.Configuration;
using Repository.Services.Seeds;

namespace Repository.Services
{
    /// <summary>
    /// 种子数据服务
    /// </summary>
    public class SeedDataService
    {
        private readonly IEnumerable<ISeedData> _seedDataServices;
        private readonly IConfiguration _configuration;

        public SeedDataService(IEnumerable<ISeedData> seedDataServices, IConfiguration configuration)
        {
            _seedDataServices = seedDataServices;
            _configuration = configuration;
        }

        /// <summary>
        /// 初始化所有种子数据
        /// </summary>
        public void InitSeedData()
        {
            // 检查是否启用种子数据生成
            bool seedDataEnabled = _configuration.GetValue<bool>("SeedData:Enabled", false);
            if (!seedDataEnabled)
            {
                Console.WriteLine("种子数据生成已禁用，跳过初始化");
                return;
            }
            
            try
            {
                Console.WriteLine("开始初始化种子数据...");
                
                // 获取所有种子数据服务并按特定顺序排序
                var orderedSeedDataServices = _seedDataServices
                    .OrderBy(GetSeedDataPriority)
                    .ToList();
                
                foreach (var seedData in orderedSeedDataServices)
                {
                    Console.WriteLine($"初始化 {seedData.GetType().Name}...");
                    seedData.Initialize();
                }
                
                Console.WriteLine("所有种子数据初始化完成");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化种子数据失败: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                throw;
            }
        }

        // 获取种子数据初始化优先级
        private int GetSeedDataPriority(ISeedData seedData)
        {
            var typeName = seedData.GetType().Name;
            
            // 菜单数据应该最先初始化
            if (typeName.Contains("Menu"))
                return 1;
            
            // 角色数据第二初始化
            if (typeName.Contains("Role"))
                return 2;
            
            // 用户数据第三初始化
            if (typeName.Contains("User"))
                return 3;
            
            // 其他数据最后初始化
            return 4;
        }
    }
}