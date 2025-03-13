using System.Text.Json.Serialization;

namespace DomainShared.Dto;
/// <summary>
/// 查询Dto
/// </summary>
public class RequestDto
{
    private int _pageSize;
    private int _pageIndex;
    /// <summary>
    /// 请求页数
    /// </summary>
    [JsonIgnore]
    public int PageNum
    {
        get => _pageIndex < 1 ? 1 : _pageIndex;
        set => _pageIndex = value;
    }
    /// <summary>
    /// 请求条数
    /// </summary>
    [JsonIgnore]
    public int PageSize
    {
        get => _pageSize != 0 ? _pageSize : 10;
        set => _pageSize = value;
    }
}

