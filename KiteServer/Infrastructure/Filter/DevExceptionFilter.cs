using Infrastructure.Exceptions;
using Infrastructure.Utility;
using Microsoft.AspNetCore.Mvc;

namespace Infrastructure.Filter;

/// <summary>
/// 开发环境异常过滤处理器
/// </summary>
public class DevExceptionFilter : IExceptionFilter
{
    private readonly ILogger<DevExceptionFilter> _logger;

    public DevExceptionFilter(ILogger<DevExceptionFilter> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void OnException(ExceptionContext context)
    {
        var errorInfo = HandleAndWrapException(context);
        context.Result = new ObjectResult(errorInfo);
        var logLevel = context.Exception.GetLogLevel();
        _logger.LogException(context.Exception, logLevel);
        context.ExceptionHandled = true;
        context.Exception = null;
    }

    protected virtual object HandleAndWrapException(ExceptionContext context)
    {
        var errorInfo = new AjaxResponse<object>("业务处理失败");

        var exception = TryToGetActualException(context.Exception);

        if (exception is UserFriendlyException friendlyException)
        {
            errorInfo.Messages = friendlyException.Message;
            errorInfo.Code = friendlyException.Code;
        }
        else
        {
            errorInfo.Results = false;
            errorInfo.Code = 0;
            errorInfo.Data = new { exception.Message, StackTrace = exception.StackTrace, InnerException = exception.InnerException };
        }

        return errorInfo;
    }

    protected virtual Exception TryToGetActualException(Exception exception)
    {
        if (exception is AggregateException && exception.InnerException != null)
        {
            var aggException = exception as AggregateException;

            if (aggException.InnerException is IFriendlyException)
            {
                return aggException.InnerException;
            }
        }

        return exception;
    }
}