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
    [SugarColumn(ColumnName = "create_time", ColumnDescription = "创建时间")]
    public DateTime CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    [SugarColumn(ColumnName = "update_time", ColumnDescription = "更新时间")]
    public DateTime UpdateTime { get; set; }

    /// <summary>
    /// 创建者
    /// </summary>
    [SugarColumn(ColumnName = "create_by", ColumnDescription = "创建者")]
    public string CreateBy { get; set; }

    /// <summary>
    /// 更新者
    /// </summary>
    [SugarColumn(ColumnName = "update_by", ColumnDescription = "更新者")]
    public string UpdateBy { get; set; }

    /// <summary>
    /// 是否删除
    /// </summary>
    [SugarColumn(ColumnName = "is_deleted")]
    public bool IsDeleted { get; set; }
}