using Application.Commands.System.Post;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Post
{
    public class UpdatePostStatusCommandHandler : IRequestHandler<UpdatePostStatusCommand, bool>
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public UpdatePostStatusCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdatePostStatusCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var post = await context.Posts.GetFirstAsync(x => x.Id == request.Id && !x.IsDeleted)
                    ?? throw new UserFriendlyException($"未找到ID为{request.Id}的岗位");

                if (post.Status == request.Status)
                    return true;

                post.Status = request.Status;
                post.UpdateBy = _currentUser.LoginName ?? "system";
                
                await context.Posts.UpdateAsync(post);
                context.Commit();

                return true;
            }
        }
    }
} 