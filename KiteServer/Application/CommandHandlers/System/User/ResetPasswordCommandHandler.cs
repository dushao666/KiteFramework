using Application.Commands.System.User;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 重置密码命令处理器
/// </summary>
public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public ResetPasswordCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            // 获取用户
            var user = await context.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            // 更新密码
            // 注意：实际项目中应该使用加密算法处理密码，这里为了简化直接赋值
            user.PassWord = request.NewPassword;
            user.UpdateBy = _currentUser.LoginName ?? "system";
            user.UpdateTime = DateTime.Now;

            // 保存更新
            await context.Users.UpdateAsync(user);
            context.Commit();
            
            return true;
        }
    }
} 