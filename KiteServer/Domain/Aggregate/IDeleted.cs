namespace Domain.Aggregate;
/// <summary>
/// 删除接口
/// </summary>
public interface IDeleted
{
    /// <summary>
    /// 是否删除
    /// </summary>
    public bool IsDeleted { get; set; }
}
