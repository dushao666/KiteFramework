using Application.Command.FileAndFolder;

namespace Api.Controllers
{
    /// <summary>
    /// 文件管理
    /// </summary>
    [Authorize]
    [ApiController, Route("api/file")]
    public class FilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public FilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        [HttpPost("save-file")]
        [ProducesResponseType(typeof(AjaxResponse<string>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveFileAsync([FromForm] SaveFileCommand command)
        {
            return new JsonResult(await _mediator.Send(command));
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        [HttpDelete("delete-file/{fileId}")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFile(long fileId)
        {
            var command = new DeleteFileCommand { FileId = fileId };
            var result = await _mediator.Send(command);

            return Ok(new AjaxResponse<bool>(result));
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [HttpGet("download-file/{filePath}")]
        [ProducesResponseType(typeof(FileStreamResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status206PartialContent)]
        [ProducesResponseType(StatusCodes.Status416RequestedRangeNotSatisfiable)]
        public async Task<IActionResult> DownloadFile(string filePath)
        {
            var command = new DownloadFileCommand
            {
                FilePath = filePath,
                RangeHeader = Request.Headers.Range.ToString()
            };

            return await _mediator.Send(command);
        }
    }
}