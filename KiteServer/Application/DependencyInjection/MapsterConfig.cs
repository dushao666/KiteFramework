using Application.Commands.System.Role;
using Application.Queries.System.Role;
using Domain.System;
using DomainShared.Dto.System;
using Mapster;

namespace Application.DependencyInjection;

/// <summary>
/// Mapster配置
/// </summary>
public static class MapsterConfig
{
    /// <summary>
    /// 注册映射关系
    /// </summary>
    public static void RegisterMappings(TypeAdapterConfig config)
    {
        // ... existing code ...

        // 角色映射
        config.NewConfig<Role, RoleDto>();
        config.NewConfig<AddRoleCommand, Role>();
        config.NewConfig<UpdateRoleCommand, Role>();
    }
} 