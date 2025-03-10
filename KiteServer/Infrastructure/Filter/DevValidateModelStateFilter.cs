using Infrastructure.Exceptions;

namespace Infrastructure.Filter;

/// <summary>
/// 开发环境模型验证
/// </summary>
public class DevValidateModelStateFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (context.ModelState.IsValid) await next();

        if (!context.ModelState.IsValid)
        {
            var validationErrors = new List<ValidationErrorInfo>();
            foreach (var state in context.ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    validationErrors.Add(new ValidationErrorInfo(error.ErrorMessage, state.Key));
                }
            }
            string msg = string.Join(";", validationErrors.Select(x => x.Message));

            throw new MissingException(msg);
        }
    }
}

internal class ValidationErrorInfo
{
    /// <summary>
    /// Validation error message.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Relate invalid members (fields/properties).
    /// </summary>
    public string[] Members { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorInfo"/>.
    /// </summary>
    public ValidationErrorInfo()
    {

    }

    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorInfo"/>.
    /// </summary>
    /// <param name="message">Validation error message</param>
    public ValidationErrorInfo(string message)
    {
        Message = message;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorInfo"/>.
    /// </summary>
    /// <param name="message">Validation error message</param>
    /// <param name="members">Related invalid members</param>
    public ValidationErrorInfo(string message, string[] members)
        : this(message)
    {
        Members = members;
    }

    /// <summary>
    /// Creates a new instance of <see cref="ValidationErrorInfo"/>.
    /// </summary>
    /// <param name="message">Validation error message</param>
    /// <param name="member">Related invalid member</param>
    public ValidationErrorInfo(string message, string member)
        : this(message, new[] { member })
    {

    }
}
