using DomainShared.Dto.System;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Repository.Repositories;

namespace Application.Queries.System.Post
{
    /// <summary>
    /// 岗位查询实现
    /// </summary>
    public class PostQueries : IPostQueries
    {
        private readonly IServiceProvider _serviceProvider;
        
        public PostQueries(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        public async Task<List<PostDto>> GetPostListAsync(PostDto model)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                
                using (var context = unitOfWork.CreateContext())
                {
                    var db = context.Posts.Context;
                    
                    var query = db.Queryable<Domain.System.Post>()
                        .Where(x => !x.IsDeleted);
                    
                    if (!string.IsNullOrEmpty(model.Name))
                    {
                        query = query.Where(x => x.Name.Contains(model.Name));
                    }
                    
                    // if (model.Status)
                    // {
                    //     query = query.Where(x => x.Status == status.Value);
                    // }
                    
                    var posts = await query
                        .OrderBy(x => x.Sort)
                        .ToListAsync();
                    
                    return mapper.Map<List<PostDto>>(posts);
                }
            }
        }

        /// <summary>
        /// 获取岗位详情
        /// </summary>
        public async Task<PostDto> GetPostDetailAsync(long id)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                
                using (var context = unitOfWork.CreateContext())
                {
                    var post = await context.Posts
                        .GetFirstAsync(x => x.Id == id && !x.IsDeleted);
                    
                    if (post == null)
                        return null;
                    
                    return mapper.Map<PostDto>(post);
                }
            }
        }
    }
} 