using Api.Middleware;
using EasyCaching.Core;
using System.Security.Claims;
using Application.Commands.System.User;

namespace Api.Controllers.System;

/// <summary>
/// 登录接口
/// </summary>
[ApiController, Route("api/login")]
public class LoginController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IConfiguration _configuration;
    private readonly IEasyCachingProvider _cacheProvider;

    public LoginController(IMediator mediator, IConfiguration configuration,
        IEasyCachingProviderFactory providerFactory)
    {
        _mediator = mediator;
        _configuration = configuration;
        _cacheProvider = configuration.GetValue<bool>("Redis:Enabled")
            ? providerFactory.GetCachingProvider("redis")
            : providerFactory.GetCachingProvider("default");
    }

    /// <summary>
    /// 用户登录
    /// </summary>
    [HttpPost]
    [Route("signIn")]
    [ProducesResponseType(typeof(AjaxResponse<LoginUserDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SigninAsync([FromBody] SignInCommand command)
    {
        var model = await _mediator.Send(command);

        var cacheKey = model.UserId + "_Identity";
        var cache = await _cacheProvider.GetAsync<LoginUserDto>(cacheKey);
        if (cache.HasValue)
        {
            return new JsonResult(cache.Value);
        }
        else
        {
            var refreshToken = EncryptionHelper.Sha512(SnowflakeId.Default().NextId().ToString());

            model.BearerToken = CreateToken(model.UserId, model.UserName, model.LoginName);
            model.RefreshToken = refreshToken;

            await _cacheProvider.SetAsync(cacheKey, model, TimeSpan.FromDays(1));
            await _cacheProvider.SetAsync(refreshToken, model.UserId, TimeSpan.FromDays(7));

            return new JsonResult(model);
        }
    }
    
    /// <summary>
    /// 退出登录
    /// </summary>
    [HttpPost]
    [Route("signOut")]
    [ProducesResponseType(typeof(AjaxResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SignOutAsync()
    {
        try
        {
            // 从Claims中获取用户ID
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.SerialNumber)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                var cacheKey = userId + "_Identity";
                await _cacheProvider.RemoveAsync(cacheKey);
            }

            return new JsonResult(AjaxResponse.Successed());
        }
        catch (Exception ex)
        {
            // 记录异常并返回成功（即使清理失败，客户端仍然可以认为退出成功）
            Console.WriteLine($"退出登录时发生错误: {ex.Message}");
            return new JsonResult(AjaxResponse.Successed());
        }
    }

    /// <summary>
    /// 生成Token
    /// </summary>
    private string CreateToken(string userId, string userName, string loginName)
    {
        var secrteKey = _configuration["Authentication:SecretKey"];
        var issuer = _configuration["Authentication:Issuer"];
        var audience = _configuration["Authentication:Audience"];
        var token = JwtHelper.GenerateToken(new List<Claim>
        {
            new Claim(ClaimTypes.SerialNumber, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.NameIdentifier, loginName)
        }, secrteKey, issuer, audience, DateTime.UtcNow.AddDays(1));
        return token;
    }
}