using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Api.Middleware;

/// <summary>
/// Jwt生成Token
/// </summary>
public static class JwtHelper
{
    private static readonly string _secrteKey = "5N90F4AnyyiOHbcZt43sU9BIqLrVYUIg";
    private static readonly string _issuer = "Certificate_Authority";
    private static readonly string _audience = "Certificate_Application";
    private static readonly DateTime? _expires = TimeExtension.NewTimeZone().AddDays(1);
    /// <summary>
    /// 生成Token
    /// </summary>
    public static string GenerateToken(IEnumerable<Claim> claimList = null, string secrteKey = "", string issuer = "", string audience = "", DateTime? expires = null)
    {
        //默认密钥
        secrteKey = string.IsNullOrWhiteSpace(secrteKey) ? _secrteKey : secrteKey;
        //默认颁发者
        issuer = string.IsNullOrWhiteSpace(issuer) ? _issuer : issuer;
        //默认使用者
        audience = string.IsNullOrWhiteSpace(audience) ? _audience : audience;
        //默认过期时间
        expires = expires == null || !expires.HasValue ? _expires : expires;

        //私钥
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secrteKey));
        //数字签名
        var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.SerialNumber,"userid"),
            new Claim("na","username")
        };
        //生成Token
        var Token = new JwtSecurityToken(issuer, audience, (claimList.Any() ? claimList : claims), DateTime.UtcNow, expires, signingCredentials);
        var token = new JwtSecurityTokenHandler().WriteToken(Token);
        return token;
    }
}