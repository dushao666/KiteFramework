namespace Infrastructure.Extension;

/// <summary>
/// 常规日志扩展类
/// </summary>
public static class CustomLoggerExtensions
{
    public static void LogWithLevel(this ILogger logger, LogLevel logLevel, string message)
    {
        switch (logLevel)
        {
            case LogLevel.Critical:
                logger.LogCritical(message);
                break;
            case LogLevel.Error:
                logger.LogError(message);
                break;
            case LogLevel.Warning:
                logger.LogWarning(message);
                break;
            case LogLevel.Information:
                logger.LogInformation(message);
                break;
            case LogLevel.Trace:
                logger.LogTrace(message);
                break;
            default: // LogLevel.Debug || LogLevel.None
                logger.LogDebug(message);
                break;
        }
    }

    public static void LogWithLevel(this ILogger logger, LogLevel logLevel, string message, Exception exception)
    {
        switch (logLevel)
        {
            case LogLevel.Critical:
                logger.LogCritical(exception, message);
                break;
            case LogLevel.Error:
                logger.LogError(exception, message);
                break;
            case LogLevel.Warning:
                logger.LogWarning(exception, message);
                break;
            case LogLevel.Information:
                logger.LogInformation(exception, message);
                break;
            case LogLevel.Trace:
                logger.LogTrace(exception, message);
                break;
            default: // LogLevel.Debug || LogLevel.None
                logger.LogDebug(exception, message);
                break;
        }
    }

    public static void LogException(this ILogger logger, Exception ex, LogLevel? level = null)
    {
        var selectedLevel = level ?? ex.GetLogLevel();
        logger.LogWithLevel(selectedLevel, ex.Message, ex);
    }
}