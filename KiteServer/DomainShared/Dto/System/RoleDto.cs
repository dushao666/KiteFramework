namespace DomainShared.Dto.System;

/// <summary>
/// 角色数据传输对象
/// </summary>
public class RoleDto : RequestDto
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public long? Id { get; set; }

    /// <summary>
    /// 角色名称
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 角色编码
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 角色描述
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// 状态（0正常 1停用）
    /// </summary>
    public int? Status { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdateTime { get; set; }
} 