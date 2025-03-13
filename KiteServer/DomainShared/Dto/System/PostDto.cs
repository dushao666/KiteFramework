namespace DomainShared.Dto.System
{
    /// <summary>
    /// 岗位数据传输对象
    /// </summary>
    public class PostDto : RequestDto
    {
        /// <summary>
        /// 岗位ID
        /// </summary>
        public long? Id { get; set; }

        /// <summary>
        /// 岗位编码
        /// </summary>
        public string? Code { get; set; }

        /// <summary>
        /// 岗位名称
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 状态（0正常 1停用）
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string? Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime? UpdateTime { get; set; }
    }
}