using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Extension;

public interface ICurrentUser
{
    /// <summary>
    /// 是否鉴权
    /// </summary>
    bool IsAuthenticated { get; }
    /// <summary>
    /// UserId
    /// </summary>
    long? UserId { get; }
    /// <summary>
    /// 登录名
    /// </summary>
    string? LoginName { get; }
}

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _contextAccessor;

    public bool IsAuthenticated => UserId.HasValue;

    public long? UserId
    {
        get
        {
            string text = _contextAccessor?.HttpContext?.User.Claims.FirstOrDefault((Claim x) => x.Type == ClaimTypes.SerialNumber)?.Value;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }

            return long.Parse(text);
        }
    }

    public string? LoginName
    {
        get
        {
            string text = _contextAccessor?.HttpContext?.User.Claims.FirstOrDefault((Claim x) => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(text)) return null;
            return text;
        }
    }

    public CurrentUser(IHttpContextAccessor contextAccessor)
    {
        _contextAccessor = contextAccessor;
    }
}