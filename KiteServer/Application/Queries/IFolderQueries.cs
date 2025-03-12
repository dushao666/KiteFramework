using DomainShared.Dto.FileAndFolderInfo;

namespace Application.Queries
{
    
    /// <summary>
    /// 文件夹查询接口
    /// </summary>
    public interface IFolderQueries
    {
        /// <summary>
        /// 根据用户ID获取文件夹树
        /// </summary>
        Task<List<FolderDto>> GetFolderTreeAsync();

        /// <summary>
        /// 获取单个文件夹详情
        /// </summary>
        Task<FolderDetailsDto> GetFolderDetailsAsync(long folderId);
        /// <summary>
        /// 文件搜索
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        Task<List<FolderDto>> GetFolderTreeAsync(FolderSearchDto search = null);
    }
}