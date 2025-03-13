using Application.Commands.System.Role;
using Application.Queries.System.Role;
using DomainShared.Dto;
using DomainShared.Dto.System;
using Infrastructure.Utility;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

/// <summary>
/// 角色管理接口
/// </summary>
[ApiController]
[Route("api/system/role")]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public RoleController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(AjaxResponse<PagedList<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleList([FromQuery] RoleDto queryDto)
    {
        var query = new GetRoleListQuery { QueryDto = queryDto };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<RoleDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleDetail(long id)
    {
        var query = new GetRoleDetailQuery { Id = id };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AjaxResponse<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddRole([FromBody] AddRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteRole(long id)
    {
        var command = new DeleteRoleCommand { Id = id };
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 更新角色状态
    /// </summary>
    [HttpPut("status")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateRoleStatus([FromBody] UpdateRoleStatusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    /// <summary>
    /// 获取角色菜单
    /// </summary>
    [HttpGet("menus/{roleId}")]
    [ProducesResponseType(typeof(AjaxResponse<List<long>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRoleMenus(long roleId)
    {
        var query = new GetRoleMenusQuery { RoleId = roleId };
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// 分配角色菜单
    /// </summary>
    [HttpPut("menus")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignRoleMenus([FromBody] AssignRoleMenusCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
} 