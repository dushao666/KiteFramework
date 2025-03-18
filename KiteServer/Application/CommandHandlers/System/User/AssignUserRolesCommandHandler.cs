using Application.Commands.System.User;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 分配用户角色命令处理器
/// </summary>
public class AssignUserRolesCommandHandler : IRequestHandler<AssignUserRolesCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AssignUserRolesCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Users.Context;
            
            // 检查用户是否存在
            var user = await context.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            // 验证角色是否存在
            if (request.RoleIds.Any())
            {
                var roleCount = await db.Queryable<Domain.System.Role>()
                    .CountAsync(r => request.RoleIds.Contains(r.Id) && !r.IsDeleted);
                    
                if (roleCount != request.RoleIds.Count)
                {
                    throw new UserFriendlyException("存在无效的角色ID");
                }
            }

            // 删除原有的用户角色关联
            await context.UserRoles.DeleteAsync(ur => ur.UserId == request.UserId);

            // 如果有新的角色ID，则添加新的关联
            if (request.RoleIds.Any())
            {
                var userRoles = request.RoleIds.Select(roleId => new UserRole
                {
                    UserId = request.UserId,
                    RoleId = roleId,
                    CreateBy = _currentUser.LoginName ?? "system",
                    UpdateBy = _currentUser.LoginName ?? "system"
                }).ToList();

                await context.UserRoles.InsertRangeAsync(userRoles);
            }

            // 更新用户的更新时间和更新人
            user.UpdateBy = _currentUser.LoginName ?? "system";
            user.UpdateTime = DateTime.Now;
            await context.Users.UpdateAsync(user);
            
            // 提交事务
            context.Commit();
            
            return true;
        }
    }
} 