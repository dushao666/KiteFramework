using SqlSugar;

namespace Domain.Aggregate;

/// <summary>
/// 领域对象基类
/// </summary>
public abstract class Entity : IDeleted
{
    /// <summary>
    /// 主键
    /// </summary>
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true, ColumnName = "id")]
    public long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    [SugarColumn(ColumnName = "create_time")]
    public DateTime CreateTime { get; set; }

    [SugarColumn(ColumnName = "update_time")]
    public DateTime UpdateTime { get; set; }

    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; }
}