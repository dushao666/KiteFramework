namespace DomainShared.Dto;
/// <summary>
/// 刷新Token
/// </summary>
public class RefreshTokenDto
{
    /// <summary>
    /// Token
    /// </summary>
    public string Token { get; set; }
    /// <summary>
    /// 刷新Token
    /// </summary>
    public string RefreshToken { get; set; }
}
