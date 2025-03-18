using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 分配用户角色命令
    /// </summary>
    public class AssignUserRolesCommand : IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public long UserId { get; set; }

        /// <summary>
        /// 角色ID列表
        /// </summary>
        [Required(ErrorMessage = "角色ID列表不能为空")]
        public List<long> RoleIds { get; set; } = new List<long>();
    }
} 