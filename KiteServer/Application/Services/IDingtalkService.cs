namespace Application.Services;
/// <summary>
/// 钉钉接口服务
/// </summary>
public interface IDingTalkService
{
    /// <summary>
    /// 通过免登授权码获取用户信息
    /// </summary>
    [Get("/api/openapi/dingtalk/getuserinfo")]
    Task<DingTalkResult<GetUserInfoResult?>> GetUserInfo([AliasAs("agent_id")] string agentId, [AliasAs("code")] string authCode);
}
