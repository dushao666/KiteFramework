using MediatR;

namespace Application.Commands.System.Role;

/// <summary>
/// 删除角色命令
/// </summary>
public class DeleteRoleCommand : IRequest<bool>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }
} 