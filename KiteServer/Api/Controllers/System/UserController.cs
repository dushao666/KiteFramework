using Application.Commands.System.User;
using Application.Queries.System.User;
using Api.Models.System.User;
using DomainShared.Dto;
using DomainShared.Dto.System;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.System;

/// <summary>
/// 用户管理接口
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly IUserQueries _userQueries;

    public UserController(IMediator mediator, IMapper mapper, IUserQueries userQueries)
    {
        _mediator = mediator;
        _mapper = mapper;
        _userQueries = userQueries;
    }

    /// <summary>
    /// 获取用户列表
    /// </summary>
    [HttpGet("list")]
    [ProducesResponseType(typeof(AjaxResponse<List<UserDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserList([FromQuery] UserDto queryDto)
    {
        var result = await _userQueries.GetUserListAsync(queryDto);
        return new JsonResult(result);
    }

    /// <summary>
    /// 获取用户详情
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserDetail(long id)
    {
        var result = await _userQueries.GetUserDetailAsync(id);
        return new JsonResult(result);
    }

    /// <summary>
    /// 添加用户
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AjaxResponse<long>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddUser([FromBody] AddUserCommand command)
    {
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }

    /// <summary>
    /// 更新用户
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserCommand command)
    {
        command.Id = id;
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }

    /// <summary>
    /// 删除用户
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteUser(long id)
    {
        var result = await _mediator.Send(new DeleteUserCommand { Id = id });
        return new JsonResult(result);
    }

    /// <summary>
    /// 更新用户状态
    /// </summary>
    [HttpPut("status")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateUserStatus([FromBody] UpdateUserStatusCommand command)
    {
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }

    /// <summary>
    /// 重置密码
    /// </summary>
    [HttpPut("password/reset")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }

    /// <summary>
    /// 获取用户角色
    /// </summary>
    [HttpGet("roles/{userId}")]
    [ProducesResponseType(typeof(AjaxResponse<List<long>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserRoles(long userId)
    {
        var result = await _userQueries.GetUserRolesAsync(userId);
        return new JsonResult(result);
    }

    /// <summary>
    /// 分配用户角色
    /// </summary>
    [HttpPut("roles/{userId}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignUserRoles(long userId, [FromBody] UserRoleRequest request)
    {
        var command = new AssignUserRolesCommand
        {
            UserId = userId,
            RoleIds = request.RoleIds
        };
        
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }
    
    /// <summary>
    /// 获取用户岗位
    /// </summary>
    [HttpGet("posts/{userId}")]
    [ProducesResponseType(typeof(AjaxResponse<List<long>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserPosts(long userId)
    {
        var result = await _userQueries.GetUserPostsAsync(userId);
        return new JsonResult(result);
    }
    
    /// <summary>
    /// 分配用户岗位
    /// </summary>
    [HttpPut("posts/{userId}")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignUserPosts(long userId, [FromBody] UserPostRequest request)
    {
        var command = new AssignUserPostsCommand
        {
            UserId = userId,
            PostIds = request.PostIds
        };
        
        var result = await _mediator.Send(command);
        return new JsonResult(result);
    }
} 