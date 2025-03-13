namespace DomainShared.Enum.User;

public enum LoginType
{
    /// <summary>
    /// 账号密码
    /// </summary>
    [Description("账号密码")]
    Password = 1,
    /// <summary>
    /// 钉钉
    /// </summary>
    [Description("钉钉")]
    DingTalk = 2
}
