using Application.Command.FileAndFolder;
using DomainShared.Dto.FileAndFolderInfo;

namespace Api.Controllers
{
    /// <summary>
    /// 文件夹
    /// </summary>
    [Authorize]
    [ApiController, Route("api/folders")]
    public class FoldersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IFolderQueries _folderQueries;

        public FoldersController(IMediator mediator, IFolderQueries folderQueries)
        {
            _mediator = mediator;
            _folderQueries = folderQueries;
        }

        /// <summary>
        /// 保存文件夹
        /// </summary>
        [HttpPost("save-folder")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SaveFolderAsync([FromBody] SaveFolderCommand command)
        {
            var result = await _mediator.Send(command);
            return new JsonResult(new AjaxResponse<bool>(result));
        }

        /// <summary>
        /// 根据用户ID获取文件夹树
        /// </summary>
        [HttpGet("get-folderTree")]
        [ProducesResponseType(typeof(AjaxResponse<List<FolderDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFolderTreeAsync()
        {
            var result = await _folderQueries.GetFolderTreeAsync();
            return Ok(new AjaxResponse<List<FolderDto>>(result));
        }

        /// <summary>
        /// 获取单个文件夹详情
        /// </summary>
        [HttpGet("{folderId}")]
        [ProducesResponseType(typeof(AjaxResponse<FolderDetailsDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFolderDetailsAsync(long folderId)
        {
            var result = await _folderQueries.GetFolderDetailsAsync(folderId);
            return Ok(new AjaxResponse<FolderDetailsDto>(result));
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderId"></param>
        /// <returns></returns>
        [HttpDelete("delete-folder/{folderId}")]
        [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteFolder(long folderId)
        {
            var command = new DeleteFolderCommand { FolderId = folderId };
            var result = await _mediator.Send(command);

            return Ok(new AjaxResponse<bool>(result));
        }


        /// <summary>
        /// 搜索文件
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        [HttpGet("search-file")]
        [ProducesResponseType(typeof(AjaxResponse<List<FolderDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchAsync([FromQuery] FolderSearchDto search)
        {
            var result = await _folderQueries.GetFolderTreeAsync(search);
            return Ok(new AjaxResponse<List<FolderDto>>(result));
        }
    }
}