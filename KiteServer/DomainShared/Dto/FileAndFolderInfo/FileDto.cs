namespace DomainShared.Dto.FileAndFolderInfo
{
    public class FileDto
    {
        /// <summary>
        /// 文件ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 文件全名
        /// </summary>
        public string FileFullName { get; set; }
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 文件类型（MIME 类型）
        /// </summary>
        public string FileType { get; set; }

        /// <summary>
        /// 文件的 MD5 加密值
        /// </summary>
        public string Md5Hash { get; set; }

        public long? FolderId { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}