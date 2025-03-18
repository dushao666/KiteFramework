using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Application.Commands.System.User
{
    /// <summary>
    /// 添加用户命令
    /// </summary>
    public class AddUserCommand : IRequest<long>
    {
        /// <summary>
        /// 用户名
        /// </summary>
        [Required(ErrorMessage = "用户名不能为空")]
        public string Name { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Required(ErrorMessage = "昵称不能为空")]
        public string NickName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [Required(ErrorMessage = "密码不能为空")]
        public string PassWord { get; set; }
        
        /// <summary>
        /// 状态(0正常，1禁用)
        /// </summary>
        public string Status { get; set; } = "0";

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