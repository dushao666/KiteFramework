using Microsoft.AspNetCore.Mvc;
using Infrastructure.Utility;
using Infrastructure.Exceptions;

namespace Infrastructure.Filter;

/// <summary>
/// 异常过滤处理器
/// </summary>
public class ExceptionFilter : IExceptionFilter
{
    private readonly ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
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