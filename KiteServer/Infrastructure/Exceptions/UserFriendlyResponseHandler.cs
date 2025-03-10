namespace Infrastructure.Exceptions;

using Infrastructure.Utility;
using System.Text.Json;

public class UserFriendlyResponseHandler : DelegatingHandler
{
    private readonly string _serviceName;

    public UserFriendlyResponseHandler(string serviceName)
    {
        _serviceName = serviceName ?? throw new ArgumentNullException(nameof(serviceName));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return response;
        }

        var content = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(content))
        {
            return response;
        }

        var responseObj =  JsonSerializer.Deserialize<AjaxResponse<object>>(content);
        if (responseObj == null)
        {
            throw new UserFriendlyException($"请求{_serviceName}出现异常");
        }

        if (!responseObj.Success
            && (responseObj.Code == 0 || responseObj.Code == 200)
            && !string.IsNullOrWhiteSpace(responseObj.Messages))
        {
            throw new UserFriendlyException(responseObj.Messages);
        }

        return response;
    }
}