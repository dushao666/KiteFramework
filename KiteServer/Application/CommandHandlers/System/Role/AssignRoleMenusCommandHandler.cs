using Application.Commands.System.Role;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 分配角色菜单命令处理器
/// </summary>
public class AssignRoleMenusCommandHandler : IRequestHandler<AssignRoleMenusCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AssignRoleMenusCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(AssignRoleMenusCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Roles.Context;
            
            // 获取角色
            var role = await context.Roles.GetByIdAsync(request.RoleId);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            // 验证菜单是否存在
            if (request.MenuIds.Any())
            {
                var menuCount = await db.Queryable<Domain.System.Menu>()
                    .CountAsync(m => request.MenuIds.Contains(m.Id) && !m.IsDeleted);
                    
                if (menuCount != request.MenuIds.Count)
                {
                    throw new UserFriendlyException("存在无效的菜单ID");
                }
            }

            // 删除原有的角色菜单关联
            await context.RoleMenus.DeleteAsync(rm => rm.RoleId == request.RoleId);

            // 如果有新的菜单ID，则添加新的关联
            if (request.MenuIds.Any())
            {
                var roleMenus = request.MenuIds.Select(menuId => new RoleMenu
                {
                    RoleId = request.RoleId,
                    MenuId = menuId,
                    CreateBy = _currentUser.LoginName ?? "system",
                    UpdateBy = _currentUser.LoginName ?? "system"
                }).ToList();

                await context.RoleMenus.InsertRangeAsync(roleMenus);
            }

            // 更新角色的更新时间和更新人
            role.UpdateBy = _currentUser.LoginName ?? "system";
            role.UpdateTime = DateTime.Now;
            await context.Roles.UpdateAsync(role);
            
            // 提交事务
            context.Commit();
            
            return true;
        }
    }
} 