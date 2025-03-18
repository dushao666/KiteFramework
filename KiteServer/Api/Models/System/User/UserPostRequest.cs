using System.ComponentModel.DataAnnotations;

namespace Api.Models.System.User
{
    /// <summary>
    /// 用户岗位分配请求
    /// </summary>
    public class UserPostRequest
    {
        /// <summary>
        /// 岗位ID列表
        /// </summary>
        [Required(ErrorMessage = "岗位ID列表不能为空")]
        public List<long> PostIds { get; set; } = new List<long>();
    }
} 