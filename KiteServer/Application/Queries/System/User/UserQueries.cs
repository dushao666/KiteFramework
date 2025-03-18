using Domain.System;
using DomainShared.Dto;
using DomainShared.Dto.System;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;
using SqlSugar;

namespace Application.Queries.System.User
{
    /// <summary>
    /// 用户查询实现
    /// </summary>
    public class UserQueries : IUserQueries
    {
        private readonly IServiceProvider _serviceProvider;

        public UserQueries(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取用户列表
        /// </summary>
        public async Task<AjaxResponse<List<UserDto>>> GetUserListAsync(UserDto queryDto)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    
                    using (var context = unitOfWork.CreateContext())
                    {
                        var db = context.Users.Context;
                        var query = db.Queryable<Domain.System.User>()
                            .Where(u => !u.IsDeleted);

                        // 根据条件筛选
                        if (!string.IsNullOrEmpty(queryDto.Name))
                        {
                            query = query.Where(u => u.Name.Contains(queryDto.Name));
                        }

                        if (!string.IsNullOrEmpty(queryDto.Status))
                        {
                            query = query.Where(u => u.Status == queryDto.Status);
                        }

                        if (!string.IsNullOrEmpty(queryDto.Keyword))
                        {
                            query = query.Where(u => u.Name.Contains(queryDto.Keyword) || u.NickName.Contains(queryDto.Keyword));
                        }

                        // 分页查询
                        RefAsync<int> totalCount = 0;
                        var users = await query
                            .OrderByDescending(u => u.Id)
                            .ToPageListAsync(queryDto.PageNum, queryDto.PageSize, totalCount);

                        var userDtos = mapper.Map<List<UserDto>>(users);

                        // 返回结果
                        var response = new AjaxResponse<List<UserDto>>(userDtos);
                        response.Total = totalCount.Value;
                        return response;
                    }
                }
            }
            catch (Exception ex)
            {
                return new AjaxResponse<List<UserDto>>(ex.Message, false);
            }
        }

        /// <summary>
        /// 获取用户详情
        /// </summary>
        public async Task<AjaxResponse<UserDto>> GetUserDetailAsync(long id)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                    var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                    
                    using (var context = unitOfWork.CreateContext())
                    {
                        var db = context.Users.Context;
                        
                        // 获取用户
                        var user = await context.Users.GetByIdAsync(id);
                        if (user == null)
                        {
                            return new AjaxResponse<UserDto>("用户不存在", false);
                        }

                        var userDto = mapper.Map<UserDto>(user);

                        // 获取用户角色
                        var userRoles = await db.Queryable<UserRole>()
                            .Where(ur => ur.UserId == id && !ur.IsDeleted)
                            .Select(ur => ur.RoleId)
                            .ToListAsync();

                        userDto.RoleIds = userRoles;
                        
                        // 获取用户岗位
                        var userPosts = await db.Queryable<UserPost>()
                            .Where(up => up.UserId == id && !up.IsDeleted)
                            .Select(up => up.PostId)
                            .ToListAsync();

                        userDto.PostIds = userPosts;

                        return new AjaxResponse<UserDto>(userDto);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AjaxResponse<UserDto>(ex.Message, false);
            }
        }

        /// <summary>
        /// 获取用户角色
        /// </summary>
        public async Task<AjaxResponse<List<long>>> GetUserRolesAsync(long userId)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                    
                    using (var context = unitOfWork.CreateContext())
                    {
                        var db = context.Users.Context;
                        
                        // 获取用户角色
                        var userRoles = await db.Queryable<UserRole>()
                            .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                            .Select(ur => ur.RoleId)
                            .ToListAsync();

                        return new AjaxResponse<List<long>>(userRoles);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AjaxResponse<List<long>>(ex.Message, false);
            }
        }
        
        /// <summary>
        /// 获取用户岗位
        /// </summary>
        public async Task<AjaxResponse<List<long>>> GetUserPostsAsync(long userId)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                    
                    using (var context = unitOfWork.CreateContext())
                    {
                        var db = context.Users.Context;
                        
                        // 获取用户岗位
                        var userPosts = await db.Queryable<UserPost>()
                            .Where(up => up.UserId == userId && !up.IsDeleted)
                            .Select(up => up.PostId)
                            .ToListAsync();

                        return new AjaxResponse<List<long>>(userPosts);
                    }
                }
            }
            catch (Exception ex)
            {
                return new AjaxResponse<List<long>>(ex.Message, false);
            }
        }
    }
} 