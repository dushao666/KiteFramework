using Application.Commands.System.Post;
using Infrastructure.Exceptions;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.Post
{
    public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, bool>
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public CreatePostCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, IMapper mapper, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(CreatePostCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var db = context.Posts.Context;
                await ValidatePostAsync(db, request);

                var post = _mapper.Map<Domain.System.Post>(request);
                post.CreateBy = _currentUser.LoginName ?? "system";
                post.UpdateBy = _currentUser.LoginName ?? "system";
                
                await context.Posts.InsertAsync(post);
                context.Commit();

                return true;
            }
        }

        private async Task ValidatePostAsync(ISqlSugarClient db, CreatePostCommand request)
        {
            if (string.IsNullOrEmpty(request.Code))
                throw new UserFriendlyException("岗位编码不能为空");

            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("岗位名称不能为空");

            var existingPost = await db.Queryable<Domain.System.Post>()
                .FirstAsync(x => (x.Code == request.Code || x.Name == request.Name) && !x.IsDeleted);

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