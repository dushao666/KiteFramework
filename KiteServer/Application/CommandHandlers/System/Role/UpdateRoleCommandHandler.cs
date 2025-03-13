using Application.Commands.System.Role;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Role;

/// <summary>
/// 更新角色命令处理器
/// </summary>
public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateRoleCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Roles.Context;
            
            // 获取角色
            var role = await context.Roles.GetByIdAsync(request.Id);
            if (role == null)
            {
                throw new UserFriendlyException("角色不存在");
            }

            // 检查角色编码是否已存在（排除自身）
            var existingRole = await db.Queryable<Domain.System.Role>()
                .FirstAsync(r => r.Code == request.Code && r.Id != request.Id && !r.IsDeleted);
                
            if (existingRole != null)
            {
                throw new UserFriendlyException("角色编码已存在");
            }

            // 更新角色信息
            role.Name = request.Name;
            role.Code = request.Code;
            role.Description = request.Description;
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