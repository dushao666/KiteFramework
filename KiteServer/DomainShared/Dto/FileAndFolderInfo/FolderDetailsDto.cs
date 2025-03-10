namespace DomainShared.Dto.FileAndFolderInfo
{
    /// <summary>
    /// 文件夹详情DTO
    /// </summary>
    public class FolderDetailsDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ParentId { get; set; }
        public long UserId { get; set; }
    }
}