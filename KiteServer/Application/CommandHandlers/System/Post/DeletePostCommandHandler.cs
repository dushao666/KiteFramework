using Application.Commands.System.Post;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Post
{
    public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand, bool>
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeletePostCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeletePostCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var post = await context.Posts.GetFirstAsync(x => x.Id == request.Id && !x.IsDeleted)
                    ?? throw new UserFriendlyException($"未找到ID为{request.Id}的岗位");

                // 检查是否有用户关联此岗位
                // 如果有关联关系，可以在这里添加检查代码
                
                // 更新岗位的更新者信息
                post.UpdateBy = _currentUser.LoginName ?? "system";
                
                // 逻辑删除
                var result = await context.Posts.DeleteAsync(post);
                context.Commit();

                return result;
            }
        }
    }
} 