using Application.Dtos;
using Microsoft.AspNetCore.Mvc;
using Repository.Services;

namespace Api.Controllers.System
{
    /// <summary>
    /// 种子数据控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SeedDataController : ControllerBase
    {
        private readonly SeedDataService _seedDataService;

        public SeedDataController(SeedDataService seedDataService)
        {
            _seedDataService = seedDataService;
        }

        /// <summary>
        /// 初始化菜单种子数据
        /// </summary>
        [HttpPost("init-menu")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public IActionResult InitMenuData()
        {
            try
            {
                _seedDataService.InitSeedData();
                return new JsonResult(new AjaxResponse<bool>(true));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"初始化菜单数据失败: {ex.Message}");
                return new JsonResult(new AjaxResponse<bool>(ex.Message));
            }
        }
    }
}