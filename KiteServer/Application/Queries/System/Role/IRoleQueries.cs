using DomainShared.Dto.System;

namespace Application.Queries.System.Role
{
    /// <summary>
    /// 角色查询接口
    /// </summary>
    public interface IRoleQueries
    {
        /// <summary>
        /// 获取角色列表
        /// </summary>
        /// <param name="model">查询条件</param>
        /// <returns>角色列表和总数</returns>
        Task<AjaxResponse<List<RoleDto>>> GetRoleListAsync(RoleDto model);

        /// <summary>
        /// 获取角色详情
        /// </summary>
        /// <param name="id">角色ID</param>
        /// <returns>角色详情</returns>
        Task<AjaxResponse<RoleDto>> GetRoleDetailAsync(long id);
        
        /// <summary>
        /// 获取角色菜单
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>菜单ID列表</returns>
        Task<AjaxResponse<List<long>>> GetRoleMenusAsync(long roleId);

        /// <summary>
        /// 获取角色权限（菜单ID列表）
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>菜单ID列表</returns>
        Task<List<long>> GetRolePermissionsAsync(long roleId);
    }
} 