namespace DomainShared.Dto;
/// <summary>
/// 登录用户Dto
/// </summary>
public class LoginUserDto
{
    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; }
    /// <summary>
    /// 登录名称
    /// </summary>
    public string LoginName { get; set; }
    /// <summary>
    /// 用户姓名
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// BearerToken
    /// </summary>
    public string BearerToken { get; set; }
    /// <summary>
    /// RefreshToken
    /// </summary>
    public string RefreshToken { get; set; }
}
