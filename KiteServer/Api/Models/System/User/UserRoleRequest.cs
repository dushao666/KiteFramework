using System.ComponentModel.DataAnnotations;

namespace Api.Models.System.User
{
    /// <summary>
    /// 用户角色分配请求
    /// </summary>
    public class UserRoleRequest
    {
        /// <summary>
        /// 角色ID列表
        /// </summary>
        [Required(ErrorMessage = "角色ID列表不能为空")]
        public List<long> RoleIds { get; set; } = new List<long>();
    }
} 