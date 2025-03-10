using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Application.Command.FileAndFolder;
using Azure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Text;

namespace Application.CommandHandler.FileAndFolder
{
    /// <summary>
    /// 下载文件命令处理器
    /// </summary>
    public class DownloadFileCommandHandler : IRequestHandler<DownloadFileCommand, IActionResult>
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DownloadFileCommandHandler> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DownloadFileCommandHandler(
            IConfiguration configuration,
            ILogger<DownloadFileCommandHandler> logger,
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            _configuration = configuration;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IActionResult> Handle(DownloadFileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 获取基础路径并规范化分隔符
                var basePath = _configuration["Upload:CommonFile"]
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar);

                // 规范化请求路径
                var normalizedRequestPath = request.FilePath
                    .Replace('/', Path.DirectorySeparatorChar)
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace("%2F", Path.DirectorySeparatorChar.ToString());

                string fullPath;
                if (_webHostEnvironment.IsDevelopment())
                {
                    // 开发环境：使用 wwwroot 路径
                    fullPath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        basePath,
                        normalizedRequestPath
                    );
                }
                else
                {
                    // 生产环境：直接使用配置的路径
                    fullPath = Path.Combine(
                        basePath,
                        normalizedRequestPath
                    );
                }

                // 规范化最终路径
                fullPath = Path.GetFullPath(fullPath);

                _logger.LogInformation("Attempting to download file from path: {FilePath}", fullPath);

                if (!File.Exists(fullPath))
                {
                    throw new FileNotFoundException($"File not found: {fullPath}");
                }

                var fileName = Path.GetFileName(request.FilePath);
                var fileInfo = new FileInfo(fullPath);
                var contentType = GetContentType(fileName);

                // 添加字符集到 Content-Type
                if (contentType.StartsWith("text/"))
                {
                    contentType = $"{contentType}; charset=utf-8";
                }

                // 处理 Range 请求
                if (!string.IsNullOrEmpty(request.RangeHeader))
                {
                    var range = GetRange(request.RangeHeader, fileInfo.Length);
                    
                    var response = _httpContextAccessor.HttpContext.Response;
                    response.Headers.Add("Accept-Ranges", "bytes");
                    response.Headers.Add("Content-Range", $"bytes {range.Start}-{range.End}/{fileInfo.Length}");
                    response.Headers.Add("Content-Disposition", $"attachment; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");
                    response.StatusCode = StatusCodes.Status206PartialContent;

                    var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
                    stream.Seek(range.Start, SeekOrigin.Begin);

                    return new FileStreamResult(stream, contentType)
                    {
                        FileDownloadName = fileName,
                        EnableRangeProcessing = true
                    };
                }

                // 非 Range 请求，返回完整文件
                var fullResponse = _httpContextAccessor.HttpContext.Response;
                fullResponse.Headers.Add("Accept-Ranges", "bytes");
                fullResponse.Headers.Add("Content-Length", fileInfo.Length.ToString());
                fullResponse.Headers.Add("Content-Disposition", $"attachment; filename*=UTF-8''{Uri.EscapeDataString(fileName)}");

                // 如果是文本文件，使用 StreamReader 确保正确的编码
                if (contentType.StartsWith("text/"))
                {
                    using (var reader = new StreamReader(fullPath, Encoding.UTF8))
                    {
                        var content = await reader.ReadToEndAsync();
                        var bytes = Encoding.UTF8.GetBytes(content);
                        return new FileContentResult(bytes, contentType)
                        {
                            FileDownloadName = fileName,
                            EnableRangeProcessing = true
                        };
                    }
                }
                
                // 非文本文件直接返回
                return new FileStreamResult(new FileStream(fullPath, FileMode.Open, FileAccess.Read), contentType)
                {
                    FileDownloadName = fileName,
                    EnableRangeProcessing = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file: {FilePath}", request.FilePath);
                throw;
            }
        }

        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".txt" => "text/plain",
                ".md" => "text/markdown",
                ".csv" => "text/csv",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                ".zip" => "application/zip",
                ".rar" => "application/x-rar-compressed",
                _ => "application/octet-stream"
            };
        }

        private (long Start, long End) GetRange(string rangeHeader, long fileLength)
        {
            try
            {
                // 解析 Range 头，格式如 "bytes=0-1023"
                var range = rangeHeader.Replace("bytes=", "").Split('-');
                var start = long.Parse(range[0]);
                var end = range.Length > 1 && !string.IsNullOrEmpty(range[1])
                    ? long.Parse(range[1])
                    : fileLength - 1;

                // 确保范围有效
                start = Math.Max(0, start);
                end = Math.Min(end, fileLength - 1);

                return (start, end);
            }
            catch (Exception)
            {
                // 如果解析失败，返回完整文件范围
                return (0, fileLength - 1);
            }
        }
    }

    /// <summary>
    /// 自定义 FileStream 包装器，用于报告下载进度
    /// </summary>
    public class ProgressReportingFileStream : Stream
    {
        private readonly FileStream _innerStream;
        private readonly IProgress<long> _progress;
        private long _bytesTransferred;
        private readonly CancellationToken _cancellationToken;

        public ProgressReportingFileStream(string filePath, IProgress<long> progress, CancellationToken cancellationToken)
        {
            _innerStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 
                bufferSize: 81920, // 使用更大的缓冲区
                useAsync: true);   // 启用异步 I/O
            _progress = progress;
            _cancellationToken = cancellationToken;
        }

        public override bool CanRead => _innerStream.CanRead;
        public override bool CanSeek => _innerStream.CanSeek;
        public override bool CanWrite => false;
        public override long Length => _innerStream.Length;

        public override long Position
        {
            get => _innerStream.Position;
            set => _innerStream.Position = value;
        }

        public override void Flush() => _innerStream.Flush();

        public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            var combinedToken = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken).Token;
            combinedToken.ThrowIfCancellationRequested();

            int bytesRead = await _innerStream.ReadAsync(buffer, offset, count, combinedToken);
            if (bytesRead > 0)
            {
                _bytesTransferred += bytesRead;
                _progress.Report(_bytesTransferred);
            }

            return bytesRead;
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            _cancellationToken.ThrowIfCancellationRequested();

            int bytesRead = _innerStream.Read(buffer, offset, count);
            if (bytesRead > 0)
            {
                _bytesTransferred += bytesRead;
                _progress.Report(_bytesTransferred);
            }

            return bytesRead;
        }

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _innerStream.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}