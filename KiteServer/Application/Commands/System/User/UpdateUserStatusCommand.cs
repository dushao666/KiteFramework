using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 更新用户状态命令
    /// </summary>
    public class UpdateUserStatusCommand : IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public long Id { get; set; }

        /// <summary>
        /// 状态(0正常，1禁用)
        /// </summary>
        [Required(ErrorMessage = "状态不能为空")]
        public string Status { get; set; }
    }
} 