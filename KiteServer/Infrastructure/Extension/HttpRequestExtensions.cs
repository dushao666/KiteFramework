using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

namespace Infrastructure.Extension;

/// <summary>
/// Http请求资源扩展类
/// </summary>
public static class HttpRequestExtensions
{
    private const string TimeoutPropertyKey = "RequestTimeout";

    public static void SetTimeout(this HttpRequestMessage request, TimeSpan? timeout)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        request.Properties[TimeoutPropertyKey] = timeout;
    }

    public static TimeSpan? GetTimeout(this HttpRequestMessage request)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        if (request.Properties.TryGetValue(TimeoutPropertyKey, out var value) && value is TimeSpan timeout)
            return timeout;
        return null;
    }

    public static IHttpClientBuilder AddHandleUserFriendlyException(this IHttpClientBuilder builder, string name)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentNullException("注册的服务名称不可为空");
        }

        builder.AddHttpMessageHandler(() => new UserFriendlyResponseHandler(name));
        return builder;
    }

    /// <summary>
    /// 从请求体中读取请求数据
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static async Task<string> GetRequestBodyAsync(this HttpRequest request)
    {
        //This line allows us to set the reader for the request back at the beginning of its stream.
        request.EnableBuffering();

        var body = request.Body;

        //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        //...Then we copy the entire request stream into the new buffer.
        await request.Body.ReadAsync(buffer, 0, buffer.Length);

        //We convert the byte[] into a string using UTF8 encoding...
        var bodyAsText = Encoding.UTF8.GetString(buffer);

        //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()

        body.Position = 0L;
        request.Body = body;

        //We convert the byte[] into a string using UTF8 encoding...
        return bodyAsText;
    }

    /// <summary>
    /// 从请求体中读取请求数据
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string GetRequestBody(this HttpRequest request)
    {
        //This line allows us to set the reader for the request back at the beginning of its stream.
        request.EnableBuffering();

        var body = request.Body;

        //We now need to read the request stream.  First, we create a new byte[] with the same length as the request stream...
        var buffer = new byte[Convert.ToInt32(request.ContentLength)];

        //...Then we copy the entire request stream into the new buffer.
        request.Body.Read(buffer, 0, buffer.Length);

        //We convert the byte[] into a string using UTF8 encoding...
        var bodyAsText = Encoding.UTF8.GetString(buffer);

        //..and finally, assign the read body back to the request body, which is allowed because of EnableRewind()

        body.Position = 0L;
        request.Body = body;

        //We convert the byte[] into a string using UTF8 encoding...
        return bodyAsText;
    }

    /// <summary>
    /// 将请求的参数转换为字典
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetRequestKeyValue(this string request)
    {
        var result = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(request))
        {
            return result;
        }

        if (request.StartsWith("?"))
        {
            request = request.Substring(1);
        }

        var splitArr = request.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
        if (splitArr.Length <= 0)
        {
            return result;
        }

        foreach (var splitInfo in splitArr)
        {
            var arr = splitInfo.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);
            if (arr.Length <= 1)
            {
                continue;
            }
            result.Add(arr[0], arr[1]);
        }
        return result;
    }
}