using Infrastructure.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Infrastructure.Filter;

/// <summary>
/// 消息执行结果处理器
/// </summary>
public class ResultFilter : IResultFilter
{
    public void OnResultExecuted(ResultExecutedContext context) { }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        switch (context)
        {
            case ResultExecutingContext resultExecutingContext:
                switch (resultExecutingContext.Result)
                {
                    case ObjectResult content:
                        if (!content.Value.GetType().Name.Equals(typeof(AjaxResponse<object>).Name))
                        {
                            var contentResp = new AjaxResponse<object>(content.Value);
                            context.Result = new ObjectResult(contentResp);
                        }
                        break;
                    case JsonResult json:
                        var resp = new AjaxResponse<object>(json.Value);
                        context.Result = new ObjectResult(resp);
                        break;
                    case EmptyResult empty:
                        context.Result = new OkObjectResult(AjaxResponse.Successed());
                        break;
                    default:
                        break;
                }
                break;
        }
    }
}