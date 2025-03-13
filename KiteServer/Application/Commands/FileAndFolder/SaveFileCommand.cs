using Microsoft.AspNetCore.Http;

namespace Application.Command.FileAndFolder
{
    /// <summary>
    /// 保存文件命令
    /// </summary>
    public class SaveFileCommand: IRequest<bool>
    {
        public long? Id { get; set; } // 文件ID（可选，用于更新）
        public IFormFile File { get; set; } // 上传的文件
        public long? FolderId { get; set; } // 所属文件夹ID
    }
}