using MediatR;

namespace Application.Commands.System.Post
{
    public class CreatePostCommand : IRequest<bool>
    {
        /// <summary>
        /// 岗位编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
} 