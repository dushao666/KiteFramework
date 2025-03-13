using MediatR;

namespace Application.Commands.System.Role;

/// <summary>
/// 分配角色菜单命令
/// </summary>
public class AssignRoleMenusCommand : IRequest<bool>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }

    /// <summary>
    /// 菜单ID列表
    /// </summary>
    public List<long> MenuIds { get; set; }
} 