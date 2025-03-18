using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 更新用户命令
    /// </summary>
    public class UpdateUserCommand : IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public long Id { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Required(ErrorMessage = "昵称不能为空")]
        public string NickName { get; set; }

        /// <summary>
        /// 钉钉用户ID
        /// </summary>
        public string DingUserId { get; set; }

        /// <summary>
        /// 状态(0正常，1禁用)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        public List<long> RoleIds { get; set; } = new List<long>();
        
        /// <summary>
        /// 岗位ID列表
        /// </summary>
        public List<long> PostIds { get; set; } = new List<long>();
    }
} 