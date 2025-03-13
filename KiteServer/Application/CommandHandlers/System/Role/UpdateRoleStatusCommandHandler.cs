using Application.Commands.System.Role;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 更新角色状态命令处理器
/// </summary>
public class UpdateRoleStatusCommandHandler : IRequestHandler<UpdateRoleStatusCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateRoleStatusCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateRoleStatusCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            // 获取角色
            var role = await context.Roles.GetByIdAsync(request.Id);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            // 更新状态
            role.Status = request.Status;
            role.UpdateBy = _currentUser.LoginName ?? "system";
            role.UpdateTime = DateTime.Now;

            // 保存更新
            await context.Roles.UpdateAsync(role);
            context.Commit();
            
            return true;
        }
    }
} 