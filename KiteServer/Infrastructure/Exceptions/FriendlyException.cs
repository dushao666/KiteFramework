namespace Infrastructure.Exceptions;

/// <summary>
/// 自定义异常
/// </summary>
[Serializable]
public class UserFriendlyException : Exception, IFriendlyException, IHasErrorCode,
        IHasLogLevel
{
    public int Code { get; set; }

    public LogLevel LogLevel { get; set; }

    public UserFriendlyException(
            string message = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning) : base(message, innerException)
    {

    }
    public UserFriendlyException(int code = 0,
            string message = null,
            Exception innerException = null,
            LogLevel logLevel = LogLevel.Warning)
            : base(message, innerException)
    {
        Code = code;
        LogLevel = logLevel;
    }
}

/// <summary>
/// 参数错误异常
/// </summary>
public class MissingException : UserFriendlyException
{
    public MissingException()
       : base(0, "请求参数有误", null, LogLevel.Information)
    {
    }

    public MissingException(string msg)
      : base(0, msg, null, LogLevel.Information)
    {
    }
}