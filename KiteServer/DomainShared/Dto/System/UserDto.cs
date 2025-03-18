
namespace DomainShared.Dto.System;

/// <summary>
/// 用户DTO
/// </summary>
public class UserDto : RequestDto
{
    /// <summary>
    /// 用户ID
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 昵称
    /// </summary>
    public string? NickName { get; set; }

    /// <summary>
    /// 密码
    /// </summary>
    public string? PassWord { get; set; }

    /// <summary>
    /// 钉钉用户ID
    /// </summary>
    public string? DingUserId { get; set; }

    /// <summary>
    /// 状态(0正常，1禁用)
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }

    /// <summary>
    /// 角色ID列表
    /// </summary>
    public List<long> RoleIds { get; set; } = new List<long>();

    /// <summary>
    /// 岗位ID列表
    /// </summary>
    public List<long> PostIds { get; set; } = new List<long>();

    /// <summary>
    /// 查询参数：关键字
    /// </summary>
    [JsonIgnore]
    public string? Keyword { get; set; }
}