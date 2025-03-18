using Application.Commands.System.User;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 删除用户命令处理器
/// </summary>
public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

    public DeleteUserCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
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
            if (user.Name == "admin")
            {
                throw new UserFriendlyException("不能删除管理员账户");
            }

            // 删除用户角色关联
            await context.UserRoles.DeleteAsync(ur => ur.UserId == request.Id);
            
            // 删除用户岗位关联
            await context.UserPosts.DeleteAsync(up => up.UserId == request.Id);

            // 删除用户
            await context.Users.DeleteAsync(user);
            
            // 提交事务
            context.Commit();
            
            return true;
        }
    }
} 