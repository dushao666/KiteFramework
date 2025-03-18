using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 重置密码命令
    /// </summary>
    public class ResetPasswordCommand : IRequest<bool>
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        [Required(ErrorMessage = "用户ID不能为空")]
        public long UserId { get; set; }

        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "新密码不能为空")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "密码长度必须在6-20个字符之间")]
        public string NewPassword { get; set; }
    }
} 