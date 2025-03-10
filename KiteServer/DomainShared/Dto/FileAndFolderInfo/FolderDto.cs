namespace DomainShared.Dto.FileAndFolderInfo
{
    /// <summary>
    /// 文件夹信息DTO
    /// </summary>
    public class FolderDto
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父文件夹ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
        public DateTime UpdateTime { get; set; }
        /// <summary>
        /// 子文件夹列表
        /// </summary>
        public List<FolderDto> Children { get; set; } = new List<FolderDto>();

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<FileDto> Files { get; set; } = new List<FileDto>();
    }
}