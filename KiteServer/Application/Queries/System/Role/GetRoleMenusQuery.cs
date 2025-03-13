using MediatR;

namespace Application.Queries.System.Role;

/// <summary>
/// 获取角色菜单查询
/// </summary>
public class GetRoleMenusQuery : IRequest<List<long>>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long RoleId { get; set; }
} 