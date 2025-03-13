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
        /// <param name="keyword">关键字搜索</param>
        /// <param name="status">状态过滤</param>
        /// <returns>岗位列表</returns>
        Task<List<PostDto>> GetPostListAsync(string? keyword = null, int? status = null);
        
        /// <summary>
        /// 获取岗位详情
        /// </summary>
        /// <param name="id">岗位ID</param>
        /// <returns>岗位详情</returns>
        Task<PostDto> GetPostDetailAsync(long id);
    }
} 