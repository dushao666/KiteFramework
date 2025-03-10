using Infrastructure.Exceptions;

namespace Infrastructure.Extension;

/// <summary>
/// 自定义异常扩展类
/// </summary>
public static class CustomeExceptionExtensions
{
    public static LogLevel GetLogLevel(this Exception exception, LogLevel defaultLevel = LogLevel.Error)
    {
        return (exception as UserFriendlyException)?.LogLevel ?? defaultLevel;
    }
}

public static class Check
{
    public static T NotNull<T>(T obj, string parameterName) where T : class
    {
        if (obj is null)
        {
            NotNull(parameterName, nameof(parameterName));

            throw new ArgumentNullException(parameterName);
        }

        return obj;
    }

    public static void NotEmptyOrNull(string parameter, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(parameter))
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}
