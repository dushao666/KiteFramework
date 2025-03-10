namespace DomainShared.Dto;

/// <summary>
/// 查询出参基类
/// </summary>
public class QueryDtoBase
{
    /// <summary>
    /// 主键
    /// </summary>
    public string Id { get; set; }
    /// <summary>
    /// 排序字段
    /// </summary>
    public int? Sort { get; set; }
    /// <summary>
    /// 描述信息
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// 是否已删除
    /// </summary>
    public bool? IsDeleted { get; set; }
}
