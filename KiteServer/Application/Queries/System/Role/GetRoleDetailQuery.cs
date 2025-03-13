using DomainShared.Dto.System;
using MediatR;

namespace Application.Queries.System.Role;

/// <summary>
/// 获取角色详情查询
/// </summary>
public class GetRoleDetailQuery : IRequest<RoleDto>
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long Id { get; set; }
} 