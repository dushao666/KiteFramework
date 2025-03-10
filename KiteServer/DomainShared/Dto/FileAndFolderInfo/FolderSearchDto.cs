namespace DomainShared.Dto.FileAndFolderInfo
{
    public class FolderSearchDto
    {
        /// <summary>
        /// 搜索关键词
        /// </summary>
        public string? Keyword { get; set; }

        /// <summary>
        /// 文件类型
        /// </summary>
        public string? FileType { get; set; }
    }
}