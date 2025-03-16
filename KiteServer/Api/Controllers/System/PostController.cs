 using Application.Commands.System.Post;
using Application.Queries.System.Post;
using DomainShared.Dto.System;

namespace Api.Controllers.System
{
    /// <summary>
    /// 岗位管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PostController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IPostQueries _postQueries;

        public PostController(IMediator mediator, IPostQueries postQueries)
        {
            _mediator = mediator;
            _postQueries = postQueries;
        }

        /// <summary>
        /// 获取岗位列表
        /// </summary>
        [HttpGet("list")]
        [ProducesResponseType(typeof(List<PostDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostList([FromQuery] PostDto model)
        {
            var result = await _postQueries.GetPostListAsync(model);
            return new JsonResult(result);
        }

        /// <summary>
        /// 获取岗位详情
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PostDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPostDetail(long id)
        {
            var result = await _postQueries.GetPostDetailAsync(id);
            if (result == null)
                return NotFound();
            
            return new JsonResult(result);
        }

        /// <summary>
        /// 添加岗位
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> AddPost([FromBody] CreatePostCommand command)
        {
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePost(long id, [FromBody] UpdatePostCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }

        /// <summary>
        /// 删除岗位
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeletePost(long id)
        {
            var result = await _mediator.Send(new DeletePostCommand { Id = id });
            return new JsonResult(result);
        }

        /// <summary>
        /// 更新岗位状态
        /// </summary>
        [HttpPut("status")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePostStatus([FromBody] UpdatePostStatusCommand command)
        {
            var result = await _mediator.Send(command);
            return new JsonResult(result);
        }
    }
}