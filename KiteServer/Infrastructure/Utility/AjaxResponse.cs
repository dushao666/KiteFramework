using System.Text.Json.Serialization;

namespace Infrastructure.Utility;

public class AjaxResponse
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("msg")]
    public string Messages { get; set; }

    [JsonPropertyName("result")]
    public bool Results { get; set; }

    [JsonPropertyName("success")]
    public bool Success { get; set; }

    public AjaxResponse()
    {
    }

    public AjaxResponse(bool success = false, string messages = null)
    {
        Results = success;
        Messages = messages;
    }

    public static AjaxResponse Successed()
    {
        return Result(isSuccess: true, 200, "请求成功");
    }

    public static AjaxResponse Failed(int code, string message = "请求失败")
    {
        return Result(isSuccess: false, code, message);
    }

    public static AjaxResponse Result(bool isSuccess, int code, string message)
    {
        return new AjaxResponse
        {
            Results = isSuccess,
            Code = code,
            Messages = message
        };
    }
}
public class AjaxResponse<TData> : AjaxResponse
{
    [JsonPropertyName("data")]
    public TData Data { get; set; }

    public AjaxResponse()
    {
    }

    public AjaxResponse(TData data)
    {
        Data = data;
        base.Results = true;
        base.Messages = "请求成功!";
        base.Code = 200;
    }

    public AjaxResponse(string messages, bool success = false)
    {
        base.Results = success;
        base.Messages = messages;
    }

    public static AjaxResponse Successed(TData data)
    {
        return Result(isSuccess: true, 200, data, "请求成功");
    }

    public static AjaxResponse Failed(int code, TData data = default(TData), string message = "请求失败")
    {
        return Result(isSuccess: false, code, data, message);
    }

    public static AjaxResponse Result(bool isSuccess, int code, TData data, string message)
    {
        return new AjaxResponse<TData>
        {
            Results = isSuccess,
            Code = code,
            Data = data,
            Messages = message
        };
    }
}
