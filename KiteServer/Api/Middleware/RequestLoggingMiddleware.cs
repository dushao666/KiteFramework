using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;

namespace Api.Middleware;

/// <summary>
/// 请求内执行结构的日志记录
/// </summary>
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;

    public RequestLoggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, ILogger<RequestLoggingMiddleware> logger)
    {
        var stopwatch = Stopwatch.StartNew();
        //First, get the incoming request
        await FormatRequest(context.Request, logger);

        //Copy a pointer to the original response body stream
        var originalBodyStream = context.Response.Body;

        //Create a new memory stream...
        using (var responseBody = new MemoryStream())
        {
            //...and use that for the temporary response body
            context.Response.Body = responseBody;

            //Continue down the Middleware pipeline, eventually returning to this class
            await _next(context);

            //Format the response from the server
            await FormatResponse(context.Request.Path, context.Response, logger);

            //Copy the contents of the new memory stream (which contains the response) to the original stream, which is then returned to the client.
            await responseBody.CopyToAsync(originalBodyStream);
        }
        logger.LogInformation($"本次接口耗时：{stopwatch.ElapsedMilliseconds} 毫秒");
        stopwatch.Stop();
    }

    private async Task FormatRequest(HttpRequest request, ILogger<RequestLoggingMiddleware> logger)
    {
        if (HttpMethods.IsPost(request.Method) ||
            HttpMethods.IsPut(request.Method) ||
            HttpMethods.IsDelete(request.Method))
        {
            // FORM请求超过200KB，不再记录
            const long MaxFormLogSize = 200 * 1024;
            if (request.ContentType != null && request.ContentType.StartsWith("multipart/form-data") && (request.ContentLength ?? 0) > MaxFormLogSize)
            {
                return;
            }

            var bodyAsText = await request.GetRequestBodyAsync();
            logger.LogInformation($"请求url：[{request.Scheme}://{request.Host}{request.Path}{request.QueryString}]-请求体：[{bodyAsText}]");
        }
        else
        {
            logger.LogInformation($"请求url：{request.Scheme}://{request.Host}{request.Path}{request.QueryString}");
        }
    }

    private async Task FormatResponse(string requestPath, HttpResponse response, ILogger<RequestLoggingMiddleware> logger)
    {
        //We need to read the response stream from the beginning...
        response.Body.Seek(0, SeekOrigin.Begin);

        //...and copy it into a string
        var text = await new StreamReader(response.Body).ReadToEndAsync();

        //We need to reset the reader for the response so that the client can read it.
        response.Body.Seek(0, SeekOrigin.Begin);

        if (!requestPath.Contains("api-docs"))
        {
            logger.LogInformation($"状态码：{response.StatusCode},响应内容：{text}");
        }
    }
}