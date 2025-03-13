using MediatR;

namespace Application.Commands.System.Role;

/// <summary>
/// 添加角色命令
/// </summary>
public class AddRoleCommand : IRequest<long>
{
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