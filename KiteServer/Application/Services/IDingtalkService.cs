namespace Application.Services;
/// <summary>
/// �����ӿڷ���
/// </summary>
public interface IDingTalkService
{
    /// <summary>
    /// ͨ�������Ȩ���ȡ�û���Ϣ
    /// </summary>
    [Get("/api/openapi/dingtalk/getuserinfo")]
    Task<DingTalkResult<GetUserInfoResult?>> GetUserInfo([AliasAs("agent_id")] string agentId, [AliasAs("code")] string authCode);
}