namespace Application.Command.FileAndFolder
{
    /// <summary>
    /// 保存文件夹命令
    /// </summary>
    public class SaveFolderCommand : IRequest<bool>
    {
        /// <summary>
        /// 文件夹ID（可选，更新时需要）
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 父文件夹ID（根文件夹为 NULL）
        /// </summary>
        public long? ParentId { get; set; }
    }
}