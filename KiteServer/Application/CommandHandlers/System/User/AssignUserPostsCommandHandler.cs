using Application.Commands.System.User;
using Domain.System;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandlers.System.User;

/// <summary>
/// 分配用户岗位命令处理器
/// </summary>
public class AssignUserPostsCommandHandler : IRequestHandler<AssignUserPostsCommand, bool>
{
    private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
    private readonly ICurrentUser _currentUser;

    public AssignUserPostsCommandHandler(ISugarUnitOfWork<DbContext> unitOfWork, ICurrentUser currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(AssignUserPostsCommand request, CancellationToken cancellationToken)
    {
        using (var context = _unitOfWork.CreateContext())
        {
            var db = context.Users.Context;
            
            // 检查用户是否存在
            var user = await context.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UserFriendlyException("用户不存在");
            }

            // 验证岗位是否存在
            if (request.PostIds.Any())
            {
                var postCount = await db.Queryable<Domain.System.Post>()
                    .CountAsync(p => request.PostIds.Contains(p.Id) && !p.IsDeleted);
                    
                if (postCount != request.PostIds.Count)
                {
                    throw new UserFriendlyException("存在无效的岗位ID");
                }
            }

            // 删除原有的用户岗位关联
            await context.UserPosts.DeleteAsync(up => up.UserId == request.UserId);
            
            // 保存新的用户岗位关联
            if (request.PostIds.Any())
            {
                var userPosts = request.PostIds.Select(postId => new UserPost
                {
                    UserId = request.UserId,
                    PostId = postId,
                    CreateBy = _currentUser.LoginName ?? "system",
                    UpdateBy = _currentUser.LoginName ?? "system"
                }).ToList();
                
                await context.UserPosts.InsertRangeAsync(userPosts);
            }
            
            context.Commit();
            return true;
        }
    }
} 