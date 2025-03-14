using Domain.System;
using DomainShared.Dto;
using DomainShared.Dto.System;
using Infrastructure.Exceptions;
using Infrastructure.Utility;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;

namespace Application.Queries.System.Role
{
    /// <summary>
    /// 角色查询实现
    /// </summary>
    public class RoleQueries : IRoleQueries
    {
        private readonly IServiceProvider _serviceProvider;

        public RoleQueries(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取角色列表
        /// </summary>
        public async Task<AjaxResponse<List<RoleDto>>> GetRoleListAsync(RoleDto model)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                using (var context = unitOfWork.CreateContext())
                {
                    var db = context.Roles.Context;
                    var query = db.Queryable<Domain.System.Role>().Where(r => !r.IsDeleted);

                    // 名称搜索
                    if (!string.IsNullOrEmpty(model.Name))
                    {
                        query = query.Where(r => r.Name.Contains(model.Name));
                    }

                    // 编码搜索
                    if (!string.IsNullOrEmpty(model.Code))
                    {
                        query = query.Where(r => r.Code.Contains(model.Code));
                    }

                    // 描述搜索
                    if (!string.IsNullOrEmpty(model.Description))
                    {
                        query = query.Where(r => r.Description.Contains(model.Description));
                    }

                    // 状态筛选
                    if (model.Status.HasValue)
                    {
                        query = query.Where(r => r.Status == model.Status.Value);
                    }

                    // 获取总数
                    var total = await query.CountAsync();

                    // 查询角色列表
                    var roles = await query
                        .OrderByDescending(r => r.CreateTime)
                        .ToListAsync();

                    // 映射为DTO
                    var roleDtos = mapper.Map<List<RoleDto>>(roles);

                    // 返回带有总数的响应
                    var response = new AjaxResponse<List<RoleDto>>(roleDtos);
                    response.Total = total;
                    return response;
                }
            }
        }

        /// <summary>
        /// 获取角色详情
        /// </summary>
        public async Task<AjaxResponse<RoleDto>> GetRoleDetailAsync(long id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                using (var context = unitOfWork.CreateContext())
                {
                    var role = await context.Roles.GetByIdAsync(id);
                    if (role == null)
                    {
                        throw new UserFriendlyException("角色不存在");
                    }

                    var roleDto = mapper.Map<RoleDto>(role);
                    return new AjaxResponse<RoleDto>(roleDto);
                }
            }
        }

        /// <summary>
        /// 获取角色菜单
        /// </summary>
        public async Task<AjaxResponse<List<long>>> GetRoleMenusAsync(long roleId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();

                using (var context = unitOfWork.CreateContext())
                {
                    var db = context.Roles.Context;
                    
                    // 检查角色是否存在
                    var role = await context.Roles.GetByIdAsync(roleId);
                    if (role == null)
                    {
                        throw new UserFriendlyException("角色不存在");
                    }

                    // 获取角色关联的菜单ID列表
                    var menuIds = await db.Queryable<Domain.System.RoleMenu>()
                        .Where(rm => rm.RoleId == roleId && !rm.IsDeleted)
                        .Select(rm => rm.MenuId)
                        .ToListAsync();

                    return new AjaxResponse<List<long>>(menuIds);
                }
            }
        }

        /// <summary>
        /// 获取角色权限（菜单ID列表）
        /// </summary>
        public async Task<List<long>> GetRolePermissionsAsync(long roleId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();

                using (var context = unitOfWork.CreateContext())
                {
                    // 检查角色是否存在
                    var role = await context.Roles.GetFirstAsync(r => r.Id == roleId && !r.IsDeleted);
                    if (role == null)
                        return new List<long>();

                    // 获取角色菜单关联
                    var menuIds = await context.RoleMenus.Context
                        .Queryable<RoleMenu>()
                        .Where(rm => rm.RoleId == roleId && !rm.IsDeleted)
                        .Select(rm => rm.MenuId)
                        .ToListAsync();

                    return menuIds;
                }
            }
        }
    }
} 