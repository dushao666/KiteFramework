using Application.Commands.System.User;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 添加用户命令处理器
/// </summary>
public class AddUserCommandHandler : IRequestHandler<AddUserCommand, long>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public AddUserCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, IMapper mapper, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<long> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Users.Context;
            
            // 检查用户名是否已存在
            var existingUser = await db.Queryable<Domain.System.User>()
                .FirstAsync(u => u.Name == request.Name && !u.IsDeleted);
                
            if (existingUser != null)
            {
                throw new UserFriendlyException("用户名已存在");
            }

            // 创建用户实体
            var user = _mapper.Map<Domain.System.User>(request);
            user.CreateBy = _currentUser.LoginName ?? "system";
            user.UpdateBy = _currentUser.LoginName ?? "system";

            // 保存用户
            await context.Users.InsertAsync(user);

            // 保存用户角色关联
            if (request.RoleIds.Any())
            {
                var userRoles = request.RoleIds.Select(roleId => new UserRole
                {
                    UserId = user.Id,
                    RoleId = roleId,
                    CreateBy = _currentUser.LoginName ?? "system",
                    UpdateBy = _currentUser.LoginName ?? "system"
                }).ToList();

                await context.UserRoles.InsertRangeAsync(userRoles);
            }

            // 保存用户岗位关联
            if (request.PostIds.Any())
            {
                var userPosts = request.PostIds.Select(postId => new UserPost
                {
                    UserId = user.Id,
                    PostId = postId,
                    CreateBy = _currentUser.LoginName ?? "system",
                    UpdateBy = _currentUser.LoginName ?? "system"
                }).ToList();

                await context.UserPosts.InsertRangeAsync(userPosts);
            }

            // 提交事务
            context.Commit();
            
            return user.Id;
        }
    }
} 