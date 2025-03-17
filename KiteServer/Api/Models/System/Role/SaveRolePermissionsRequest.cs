namespace Api.Models.System.Role
{
    /// <summary>
    /// 保存角色权限请求
    /// </summary>
    public class SaveRolePermissionsRequest
    {
        /// <summary>
        /// 菜单ID列表
        /// </summary>
        public List<long> menuIds { get; set; } = new List<long>();
    }
} 