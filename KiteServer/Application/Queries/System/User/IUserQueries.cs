using Domain.System;
using DomainShared.Dto;
using DomainShared.Dto.System;

namespace Application.Queries.System.User
{
    public interface IUserQueries
    {
        /// <summary>
        /// 获取用户列表
        /// </summary>
        Task<AjaxResponse<List<UserDto>>> GetUserListAsync(UserDto queryDto);

        /// <summary>
        /// 获取用户详情
        /// </summary>
        Task<AjaxResponse<UserDto>> GetUserDetailAsync(long id);

        /// <summary>
        /// 获取用户角色
        /// </summary>
        Task<AjaxResponse<List<long>>> GetUserRolesAsync(long userId);
        
        /// <summary>
        /// 获取用户岗位
        /// </summary>
        Task<AjaxResponse<List<long>>> GetUserPostsAsync(long userId);
    }
} 