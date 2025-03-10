namespace DomainShared.Dto.DingTalk;

public class GetUserInfoResult
{
    /// <summary>
    /// UserId
    /// </summary>
    public string OpenId { get; set; }
    /// <summary>
    /// 姓名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// UnionId
    /// </summary>
    public string UnionId { get; set; }
}
