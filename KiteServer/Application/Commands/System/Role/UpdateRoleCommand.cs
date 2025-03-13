using MediatR;
using Infrastructure.Exceptions;
using Repository.Repositories;

namespace Application.Commands.System.Role;

/// <summary>
/// 更新角色命令
/// </summary>
public class UpdateRoleCommand : IRequest<bool>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public int Status { get; set; }
}
