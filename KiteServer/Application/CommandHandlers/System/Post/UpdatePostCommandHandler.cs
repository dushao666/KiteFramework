using Application.Commands.System.Post;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Post
{
    public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand, bool>
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public UpdatePostCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(UpdatePostCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var db = context.Posts.Context;
                await ValidatePostAsync(db, request);

                var post = await context.Posts.GetByIdAsync(request.Id)
                    ?? throw new UserFriendlyException($"未找到ID为{request.Id}的岗位");

                post.Code = request.Code;
                post.Name = request.Name;
                post.Sort = request.Sort;
                post.Status = request.Status;
                post.Remark = request.Remark;
                post.UpdateBy = _currentUser.LoginName ?? "system";
                
                await context.Posts.UpdateAsync(post);
                context.Commit();

                return true;
            }
        }

        private async Task ValidatePostAsync(ISqlSugarClient db, UpdatePostCommand request)
        {
            if (string.IsNullOrEmpty(request.Code))
                throw new UserFriendlyException("岗位编码不能为空");

            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("岗位名称不能为空");

            var existingPost = await db.Queryable<Domain.System.Post>()
                .FirstAsync(x => (x.Code == request.Code || x.Name == request.Name) && x.Id != request.Id && !x.IsDeleted);

            if (existingPost != null)
            {
                if (existingPost.Code == request.Code)
                    throw new UserFriendlyException("岗位编码已存在");
                
                if (existingPost.Name == request.Name)
                    throw new UserFriendlyException("岗位名称已存在");
            }
        }
    }
} 