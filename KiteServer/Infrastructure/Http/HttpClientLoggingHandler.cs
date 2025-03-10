namespace Infrastructure.Http;

/// <summary>
/// 跨应用接口调用响应日志
/// </summary>
public class HttpClientLoggingHandler : DelegatingHandler
{
    private readonly ILogger<HttpClientLoggingHandler> _logger;

    public HttpClientLoggingHandler(ILogger<HttpClientLoggingHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        await Log.RequestPipelineStart(_logger, request);
        var response = await base.SendAsync(request, cancellationToken);
        await Log.RequestPipelineEnd(_logger, response);

        return response;
    }

    private static class Log
    {
        private static class EventIds
        {
            public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
            public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");
        }

        private static readonly Func<ILogger, HttpMethod, Uri, string, IDisposable> _beginRequestPipelineScope =
            LoggerMessage.DefineScope<HttpMethod, Uri, string>("HTTP {HttpMethod} {Uri} {CorrelationId}");

        private static readonly Action<ILogger, HttpMethod, Uri, string, string, System.Exception> _requestPipelineStart =
            LoggerMessage.Define<HttpMethod, Uri, string, string>(
                LogLevel.Information,
                EventIds.PipelineStart,
                "Start processing HTTP request {HttpMethod} {Uri} [Correlation: {CorrelationId}],[Content:{Content}]");

        private static readonly Action<ILogger, long, string, System.Exception> _requestPipelineEnd =
            LoggerMessage.Define<long, string>(
                LogLevel.Information,
                EventIds.PipelineEnd,
                "End processing HTTP request {StatusCode} [Content:{Content}]");

        public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request)
        {
            var correlationId = GetCorrelationIdFromRequest(request);
            return _beginRequestPipelineScope(logger, request.Method, request.RequestUri, correlationId);
        }

        public static async Task RequestPipelineStart(ILogger logger, HttpRequestMessage request)
        {
            var content = request.Content == null ? string.Empty : await request.Content.ReadAsStringAsync();
            var correlationId = GetCorrelationIdFromRequest(request);
            _requestPipelineStart(logger, request.Method, request.RequestUri, correlationId, content, null);
        }

        public static async Task RequestPipelineEnd(ILogger logger, HttpResponseMessage response)
        {
            var content = response.Content == null ? string.Empty : await response.Content.ReadAsStringAsync();
            _requestPipelineEnd(logger, response.StatusCode.GetHashCode(), content, null);
        }

        private static string GetCorrelationIdFromRequest(HttpRequestMessage request)
        {
            var correlationId = "Not set";

            if (request.Headers.TryGetValues("X-Correlation-ID", out var values))
            {
                correlationId = values.First();
            }

            return correlationId;
        }
    }
}