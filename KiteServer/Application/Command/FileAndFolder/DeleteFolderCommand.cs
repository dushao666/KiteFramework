namespace Application.Command.FileAndFolder
{
    public class DeleteFolderCommand: IRequest<bool>
    {
        /// <summary>
        /// 文件夹ID
        /// </summary>
        public long FolderId { get; set; }
    }
}