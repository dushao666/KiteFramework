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
[Route("api/[controller]")]
public class RoleController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IRoleQueries _roleQueries;

    public RoleController(IMediator mediator, IMapper mapper, IRoleQueries roleQueries)
    {
        _mediator = mediator;
        _mapper = mapper;
        _roleQueries = roleQueries;
    }

    /// <summary>
    /// 获取角色列表
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(AjaxResponse<List<RoleDto>>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<List<RoleDto>>> GetRoleList([FromQuery] RoleDto queryDto)
    {
        return await _roleQueries.GetRoleListAsync(queryDto);
    }

    /// <summary>
    /// 获取角色详情
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<RoleDto>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<RoleDto>> GetRoleDetail(long id)
    {
        return await _roleQueries.GetRoleDetailAsync(id);
    }

    /// <summary>
    /// 添加角色
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AjaxResponse<long>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<long>> AddRole([FromBody] AddRoleCommand command)
    {
        var result = await _mediator.Send(command);
        return new AjaxResponse<long>(result);
    }

    /// <summary>
    /// 更新角色
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<bool>> UpdateRole(long id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return new AjaxResponse<bool>(result);
    }

    /// <summary>
    /// 删除角色
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<bool>> DeleteRole(long id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand { Id = id });
        return new AjaxResponse<bool>(result);
    }

    /// <summary>
    /// 更新角色状态
    /// </summary>
    [HttpPut("status")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<bool>> UpdateRoleStatus([FromBody] UpdateRoleStatusCommand command)
    {
        var result = await _mediator.Send(command);
        return new AjaxResponse<bool>(result);
    }

    /// <summary>
    /// 获取角色菜单
    /// </summary>
    [HttpGet("menus/{roleId}")]
    [ProducesResponseType(typeof(AjaxResponse<List<long>>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<List<long>>> GetRoleMenus(long roleId)
    {
        return await _roleQueries.GetRoleMenusAsync(roleId);
    }

    /// <summary>
    /// 分配角色菜单
    /// </summary>
    [HttpPut("menus")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<AjaxResponse<bool>> AssignRoleMenus([FromBody] AssignRoleMenusCommand command)
    {
        var result = await _mediator.Send(command);
        return new AjaxResponse<bool>(result);
    }

    /// <summary>
    /// 获取角色权限
    /// </summary>
    [HttpGet("permissions/{roleId}")]
    [ProducesResponseType(typeof(AjaxResponse<List<long>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRolePermissions(long roleId)
    {
        var result = await _roleQueries.GetRolePermissionsAsync(roleId);
        return new JsonResult(result);
    }
}