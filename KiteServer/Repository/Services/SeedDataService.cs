using Repository.Services.Seeds;

namespace Repository.Services
{
    /// <summary>
    /// 种子数据服务
    /// </summary>
    public class SeedDataService
    {
        private readonly IEnumerable<ISeedData> _seedDataServices;

        public SeedDataService(IEnumerable<ISeedData> seedDataServices)
        {
            _seedDataServices = seedDataServices;
        }

        /// <summary>
        /// 初始化所有种子数据
        /// </summary>
        public void InitSeedData()
        {
            try
            {
                foreach (var seedData in _seedDataServices)
                {
                    seedData.Initialize();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化种子数据失败: {ex.Message}");
                throw;
            }
        }
    }
}