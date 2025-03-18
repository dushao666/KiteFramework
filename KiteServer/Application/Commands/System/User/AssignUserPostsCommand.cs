using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 分配用户岗位命令
    /// </summary>
    public class AssignUserPostsCommand : IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public long UserId { get; set; }

        /// <summary>
        /// 岗位ID列表
        /// </summary>
        [Required(ErrorMessage = "岗位ID列表不能为空")]
        public List<long> PostIds { get; set; } = new List<long>();
    }
} 