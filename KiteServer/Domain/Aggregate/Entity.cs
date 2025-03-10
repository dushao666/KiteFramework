using SqlSugar;

namespace Domain.Aggregate;

/// <summary>
/// 领域对象基类
/// </summary>
public abstract class Entity : IDeleted
{
    private long _id;

    /// <summary>
    /// 主键
    /// </summary>
    [Key]
    public long Id
    {
        get => _id;
        protected set => _id = value;
    }

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