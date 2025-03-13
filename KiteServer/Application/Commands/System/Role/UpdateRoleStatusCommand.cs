using MediatR;

namespace Application.Commands.System.Role;

/// <summary>
/// 更新角色状态命令
/// </summary>
public class UpdateRoleStatusCommand : IRequest<bool>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public int Status { get; set; }
} 