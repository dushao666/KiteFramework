namespace Infrastructure.Exceptions;

/// <summary>
/// 错误、异常的结构封装
/// </summary>
public interface IHasErrorCode
{
    int Code { get; }
}

public interface IHasLogLevel
{
    LogLevel LogLevel { get; set; }
}

public interface IFriendlyException
{
    int Code { get; }
    LogLevel LogLevel { get; set; }
}

