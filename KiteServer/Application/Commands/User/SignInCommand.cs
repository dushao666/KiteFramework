namespace Application.Commands.User;

public class SignInCommand : IRequest<LoginUserDto>
{
    /// <summary>
    /// 登录类型
    /// </summary>
    public LoginType Type { get; set; }
    /// <summary>
    /// 登录名称
    /// </summary>
    public string? Login { get; set; }
    /// <summary>
    /// 密码
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// 钉钉免登授权码
    /// </summary>
    public string? DingTalkAuthCode { get; set; }
}
