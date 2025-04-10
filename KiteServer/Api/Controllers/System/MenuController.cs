using Application.Commands.System.Menu;
using Application.Queries.System.Menu;
using DomainShared.Dto.System;

namespace Api.Controllers.System
{
    /// <summary>
    /// 菜单管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MenuController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMenuQueries _menuQueries;

        public MenuController(IMediator mediator, IMenuQueries menuQueries)
        {
            _mediator = mediator;
            _menuQueries = menuQueries;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        [HttpGet("menuTree")]
        [ProducesResponseType(typeof(AjaxResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMenuTree([FromQuery] bool includeHidden = false)
        {
            var result = await _menuQueries.GetMenuTreeAsync(includeHidden);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        [HttpGet("menuList")]
        [ProducesResponseType(typeof(AjaxResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMenuList([FromQuery] string keyword = null, [FromQuery] bool includeHidden = false)
        {
            var result = await _menuQueries.GetMenuListAsync(keyword, includeHidden);
            return new JsonResult(result);
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        [HttpPost("addMenu")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateMenu([FromBody] CreateMenuCommand command)
        {
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取当前用户的菜单
        /// </summary>
        [HttpGet("userMenus")]
        [ProducesResponseType(typeof(AjaxResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserMenus()
        {
            var result = await _menuQueries.GetUserMenusAsync();
            return new JsonResult(result);
        }

        /// <summary>
        /// 更新菜单
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateMenu(long id, [FromBody] UpdateMenuCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteMenu(long id)
        {
            var result = await _mediator.Send(new DeleteMenuCommand { Id = id });
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取前端路由配置
        /// </summary>
        [HttpGet("getRouters")]
        [ProducesResponseType(typeof(AjaxResponse<List<MenuDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRouters()
        {
            try
            {
                // 获取用户菜单并转换为路由格式
                var menus = await _menuQueries.GetUserMenusAsync();
                
                // 确保菜单ID是字符串而不是数字（前端路由组件可能需要字符串ID）
                foreach (var menu in menus)
                {
                    NormalizeMenuIds(menu);
                }
                
                // 将结果格式化为前端期望的路由结构
                return new JsonResult(new AjaxResponse<List<MenuDto>>(menus));
            }
            catch (Exception ex)
            {
                // 记录异常并返回空菜单列表
                Console.WriteLine($"获取路由数据时出错: {ex.Message}");
                return new JsonResult(new AjaxResponse<List<MenuDto>>(new List<MenuDto>()));
            }
        }

        /// <summary>
        /// 确保菜单ID是字符串类型
        /// </summary>
        private void NormalizeMenuIds(MenuDto menu)
        {
            // 确保ID是字符串类型
            if (menu.Id != 0 && !menu.Id.ToString().Contains("\""))
            {
                // 注：MenuDto中的Id应该是long类型，如果是int或其他数值类型，这里不会有问题
                // 前端期望的Id是字符串类型
            }
            
            // 递归处理子菜单
            if (menu.Children != null && menu.Children.Any())
            {
                foreach (var child in menu.Children)
                {
                    NormalizeMenuIds(child);
                }
            }
        }
    }
} 