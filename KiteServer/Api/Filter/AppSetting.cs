namespace Api;

/// <summary>
/// 公共配置信息
/// </summary>
public class AppSetting
{
    /// <summary>
    /// 环境变量
    /// </summary>
    public static string EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
           ?? "Development";
    /// <summary>
    /// 应用名称
    /// </summary>
    public const string Name = "WebApiService";
}
