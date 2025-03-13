using Application.Commands.Menu;
using Application.Queries.Menu;
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
    }
} 