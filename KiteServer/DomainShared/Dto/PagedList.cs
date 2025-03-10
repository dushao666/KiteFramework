namespace DomainShared.Dto;
/// <summary>
/// 分页结果
/// </summary>
public class PagedList<T>
{
    /// <summary>
    /// 分页总数
    /// </summary>
    public int TotalCount { get; set; }
    /// <summary>
    /// 分页数据
    /// </summary>
    public IEnumerable<T> Data { get; set; }
    public PagedList(int totalCount, IEnumerable<T> data)
    {
        TotalCount = totalCount;
        Data = data;
    }
}
