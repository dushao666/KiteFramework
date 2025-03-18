using Application.Commands.System.User;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 更新用户命令处理器
/// </summary>
public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateUserCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Users.Context;
            
            // 获取用户
            var user = await context.Users.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            // 更新用户信息
            user.NickName = request.NickName;
            user.DingUserId = request.DingUserId;
            user.Status = request.Status;
            user.UpdateBy = _currentUser.LoginName ?? "system";
            user.UpdateTime = DateTime.Now;

            // 保存更新
            await context.Users.UpdateAsync(user);

            // 更新用户角色关联
            // 1. 删除原有角色关联
            await context.UserRoles.DeleteAsync(ur => ur.UserId == request.Id);
            
            // 2. 添加新的角色关联
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

            // 更新用户岗位关联
            // 1. 删除原有岗位关联
            await context.UserPosts.DeleteAsync(up => up.UserId == request.Id);
            
            // 2. 添加新的岗位关联
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
            
            return true;
        }
    }
} 