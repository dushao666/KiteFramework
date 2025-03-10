using Infrastructure.Exceptions;

namespace Infrastructure.Filter;

/// <summary>
/// 生成环境模型验证
/// </summary>
public class ValidateModelStateFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid) await next();

        if (!context.ModelState.IsValid)
        {
            throw new MissingException();
        }
    }
}