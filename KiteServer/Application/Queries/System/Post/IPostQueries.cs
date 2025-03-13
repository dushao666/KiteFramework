using DomainShared.Dto.System;

namespace Application.Queries.System.Post
{
    /// <summary>
    /// 岗位查询接口
    /// </summary>
    public interface IPostQueries
    {
        /// <summary>
        /// 获取岗位列表
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<List<PostDto>> GetPostListAsync(PostDto model);

        /// <summary>
        /// 获取岗位详情
        /// </summary>
        /// <param name="id">岗位ID</param>
        /// <returns>岗位详情</returns>
        Task<PostDto> GetPostDetailAsync(long id);
    }
}