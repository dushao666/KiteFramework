using Domain.FileInfo;
using SqlSugar;

namespace Domain.FileAndFolderInfo
{
    [SugarTable("Folders", TableDescription = "文件夹表")]
    public class FolderInfos : Entity
    {
        /// <summary>
        /// 文件夹名称
        /// </summary>
        [SugarColumn(ColumnName = "name", ColumnDescription = "文件夹名称", IsNullable = false,
            ColumnDataType = "varchar(255)")]
        public string Name { get; set; }

        /// <summary>
        /// 父文件夹ID（根文件夹为 NULL）
        /// </summary>
        [SugarColumn(ColumnName = "parent_id", ColumnDescription = "父文件夹ID", IsNullable = true)]
        public long? ParentId { get; set; }

        /// <summary>
        /// 导航属性：文件
        /// </summary>
        [Navigate(NavigateType.OneToMany, nameof(FileInfos.FolderId))]
        public List<FileInfos> Files { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [SugarColumn(ColumnName = "user_id", ColumnDescription = "用户ID", IsNullable = false)]
        public long UserId { get; set; }

        public FolderInfos()
        {
        }

        public FolderInfos(long userId, string name, long? parentId, long? id)
        {
            UserId = userId;
            Name = name;
            ParentId = parentId;
            Id = SnowflakeId.Default().NextId();
        }
    }
}