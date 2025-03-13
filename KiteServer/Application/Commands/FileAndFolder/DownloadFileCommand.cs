using Microsoft.AspNetCore.Mvc;

namespace Application.Command.FileAndFolder
{
    /// <summary>
    /// 下载文件命令
    /// </summary>
    public class DownloadFileCommand : IRequest<IActionResult>
    {
        /// <summary>
        /// 文件的相对路径
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Range 请求头
        /// </summary>
        public string RangeHeader { get; set; }
    }
}
