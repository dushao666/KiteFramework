using Application.Commands.System.Role;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 删除角色命令处理器
/// </summary>
public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

    public DeleteRoleCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            // 获取角色
            var role = await context.Roles.GetByIdAsync(request.Id);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            // 删除角色菜单关联
            await context.RoleMenus.DeleteAsync(rm => rm.RoleId == request.Id);

            // 删除角色
            await context.Roles.DeleteAsync(role);
            
            // 提交事务
            context.Commit();
            
            return true;
        }
    }
} 