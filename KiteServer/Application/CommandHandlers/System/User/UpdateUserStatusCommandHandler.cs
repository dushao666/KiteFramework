using Application.Commands.System.User;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 更新用户状态命令处理器
/// </summary>
public class UpdateUserStatusCommandHandler : IRequestHandler<UpdateUserStatusCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public UpdateUserStatusCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateUserStatusCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            // 获取用户
            var user = await context.Users.GetByIdAsync(request.Id);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            // 检查是否为管理员
            if (user.Name == "admin" && request.Status != "0")
            {
                throw new UserFriendlyException("不能禁用管理员账户");
            }

            // 更新状态
            user.Status = request.Status;
            user.UpdateBy = _currentUser.LoginName ?? "system";
            user.UpdateTime = DateTime.Now;

            // 保存更新
            await context.Users.UpdateAsync(user);
            context.Commit();
            
            return true;
        }
    }
} 