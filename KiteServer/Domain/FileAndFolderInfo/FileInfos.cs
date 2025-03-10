using Domain.FileAndFolderInfo;
using SqlSugar;

namespace Domain.FileInfo
{
    [SugarTable("Files", TableDescription = "文件表")]
    public class FileInfos : Entity
    {
        /// <summary>
        /// 文件名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "文件名称", IsNullable = false,
            ColumnDataType = "varchar(255)")]
        public string Name { get; set; }

        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        [SugarColumn(ColumnName = "size", ColumnDescription = "文件大小（字节）", IsNullable = false)]
        public long Size { get; set; }

        /// <summary>
        /// 文件存储路径
        /// </summary>
        [SugarColumn(ColumnName = "path", ColumnDescription = "文件存储路径", IsNullable = false,
            ColumnDataType = "varchar(500)")]
        public string Path { get; set; }

        /// <summary>
        /// 所属文件夹ID
        /// </summary>
        [SugarColumn(ColumnName = "folder_id", ColumnDescription = "所属文件夹ID", IsNullable = true)]
        public long? FolderId { get; set; }

        /// <summary>
        /// 文件类型（MIME 类型）
        /// </summary>
        [SugarColumn(ColumnName = "file_type", ColumnDescription = "文件类型", IsNullable = false,
            ColumnDataType = "varchar(100)")]
        public string FileType { get; set; }

        /// <summary>
        /// 文件的 MD5 加密值
        /// </summary>
        [SugarColumn(ColumnName = "md5_hash", ColumnDescription = "MD5 加密值", IsNullable = false,
            ColumnDataType = "varchar(32)")]
        public string Md5Hash { get; set; }

        [SugarColumn(ColumnName = "file_name", ColumnDescription = "文件全名", IsNullable = false)]
        public string FileName { get; set; }

        /// <summary>
        /// 导航属性：所属文件夹
        /// </summary>
        [Navigate(NavigateType.ManyToOne, nameof(FolderId))]
        public FolderInfos FolderInfos { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID", IsNullable = false)]
        public long UserId { get; set; }

        public FileInfos()
        {
        }

        public FileInfos(long userId, string name, string fileName, long? folderId, long size, string path,
            string fileType, string md5Hash)
        {
            Id = SnowflakeId.Default().NextId();
            UserId = userId;
            Name = name; // 确保 Name 被正确赋值
            FileName = fileName;
            FolderId = folderId;
            Size = size;
            Path = path;
            FileType = fileType;
            Md5Hash = md5Hash;
        }
    }
}