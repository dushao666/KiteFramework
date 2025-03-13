namespace Application.Command.FileAndFolder
{
    public class DeleteFileCommand: IRequest<bool>
    {
        
        /// <summary>
        /// 文件ID
        /// </summary>
        public long FileId { get; set; }
    }
}