using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Renci.SshNet;

namespace Infrastructure.Services
{
    public interface ISftpService
    {
        Task UploadFileAsync(string localPath, string remotePath);
        Task<Stream> DownloadFileAsync(string remotePath);
        bool FileExists(string remotePath);
    }

    public class SftpService : ISftpService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<SftpService> _logger;

        public SftpService(IConfiguration configuration, IWebHostEnvironment environment, ILogger<SftpService> logger)
        {
            _configuration = configuration;
            _environment = environment;
            _logger = logger;
        }

        public async Task UploadFileAsync(string localPath, string remotePath)
        {
            await Task.Delay(1);
            var config = _configuration.GetSection("Upload:RemoteServer");
            if (!config.GetValue<bool>("Enabled"))
            {
                return;
            }

            var keyFile = Path.Combine(_environment.ContentRootPath, config["KeyFile"]);
            
            // 检查密钥文件是否存在
            if (!File.Exists(keyFile))
            {
                _logger.LogError("SSH key file not found at {KeyFile}", keyFile);
                throw new FileNotFoundException($"SSH key file not found at {keyFile}");
            }

            _logger.LogInformation("Attempting to connect to SFTP server {Host}:{Port} with key file {KeyFile}", 
                config["Host"], 
                config.GetValue<int>("Port"), 
                keyFile);

            SftpClient client = null;
            try
            {
                var connectionInfo = new ConnectionInfo(
                    config["Host"],
                    config.GetValue<int>("Port"),
                    config["Username"],
                    new PrivateKeyAuthenticationMethod(config["Username"], new PrivateKeyFile(keyFile))
                );

                client = new SftpClient(connectionInfo);
                client.Connect();
                _logger.LogInformation("Connected to SFTP server");

                // 规范化远程路径
                remotePath = remotePath.Replace("\\", "/");
                if (!remotePath.StartsWith("/"))
                {
                    remotePath = "/" + remotePath;
                }

                _logger.LogInformation("Normalized remote path: {RemotePath}", remotePath);

                // 确保基础目录存在
                var baseUploadPath = config["RemotePath"].TrimEnd('/');
                _logger.LogInformation("Base upload path: {BaseUploadPath}", baseUploadPath);
                
                if (!string.IsNullOrEmpty(baseUploadPath))
                {
                    if (!client.Exists(baseUploadPath))
                    {
                        _logger.LogInformation("Creating base upload directory: {Directory}", baseUploadPath);
                        CreateDirectoryRecursively(client, baseUploadPath);
                    }
                }

                // 获取目标目录路径
                var remoteDir = Path.GetDirectoryName(remotePath)?.Replace("\\", "/");
                _logger.LogInformation("Remote directory: {RemoteDir}", remoteDir);
                
                if (!string.IsNullOrEmpty(remoteDir))
                {
                    EnsureRemoteDirectoryExists(client, remoteDir);
                }

                // 检查本地文件是否存在
                if (!File.Exists(localPath))
                {
                    _logger.LogError("Local file not found at {LocalPath}", localPath);
                    throw new FileNotFoundException($"Local file not found at {localPath}");
                }

                // 上传文件
                using var fileStream = File.OpenRead(localPath);
                client.UploadFile(fileStream, remotePath);
                
                _logger.LogInformation("File uploaded successfully to {RemotePath}", remotePath);

                // 验证文件是否成功上传
                if (!client.Exists(remotePath))
                {
                    throw new Exception($"File upload verification failed. File not found at {remotePath}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to SFTP server. Local path: {LocalPath}, Remote path: {RemotePath}", 
                    localPath, 
                    remotePath);
                throw;
            }
            finally
            {
                if (client?.IsConnected == true)
                {
                    client.Disconnect();
                }
                client?.Dispose();
            }
        }

        private void EnsureRemoteDirectoryExists(SftpClient client, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            _logger.LogInformation("Ensuring directory exists: {Path}", path);

            // 规范化路径，确保使用正斜杠
            path = path.Replace("\\", "/").TrimStart('/');
            var fullPath = "/";

            // 逐级创建目录
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                fullPath = $"{fullPath}{segment}/";
                try
                {
                    _logger.LogInformation("Checking directory: {Directory}", fullPath);
                    if (!client.Exists(fullPath))
                    {
                        _logger.LogInformation("Creating directory: {Directory}", fullPath);
                        client.CreateDirectory(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create directory {Directory}", fullPath);
                    throw;
                }
            }
        }

        private void CreateDirectoryRecursively(SftpClient client, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            _logger.LogInformation("Creating directory recursively: {Path}", path);

            // 规范化路径，确保使用正斜杠
            path = path.Replace("\\", "/").TrimStart('/');
            var fullPath = "/";

            // 逐级创建目录
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            foreach (var segment in segments)
            {
                fullPath = $"{fullPath}{segment}/";
                try
                {
                    _logger.LogInformation("Checking directory: {Directory}", fullPath);
                    if (!client.Exists(fullPath))
                    {
                        _logger.LogInformation("Creating directory: {Directory}", fullPath);
                        client.CreateDirectory(fullPath);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to create directory {Directory}", fullPath);
                    throw;
                }
            }
        }

        public async Task<Stream> DownloadFileAsync(string remotePath)
        {
            var config = _configuration.GetSection("Upload:RemoteServer");
            if (!config.GetValue<bool>("Enabled"))
            {
                throw new InvalidOperationException("Remote server is not enabled");
            }

            var keyFile = Path.Combine(_environment.ContentRootPath, config["KeyFile"]);
            
            if (!File.Exists(keyFile))
            {
                throw new FileNotFoundException($"SSH key file not found at {keyFile}");
            }

            try
            {
                var connectionInfo = new ConnectionInfo(
                    config["Host"],
                    config.GetValue<int>("Port"),
                    config["Username"],
                    new PrivateKeyAuthenticationMethod(config["Username"], new PrivateKeyFile(keyFile))
                );

                var client = new SftpClient(connectionInfo);
                await Task.Run(() => client.Connect());

                // 规范化路径
                remotePath = remotePath.Replace("\\", "/");
                if (!remotePath.StartsWith("/"))
                {
                    remotePath = "/" + remotePath;
                }

                if (!client.Exists(remotePath))
                {
                    throw new FileNotFoundException($"File not found on remote server: {remotePath}");
                }

                var memoryStream = new MemoryStream();
                client.DownloadFile(remotePath, memoryStream);
                memoryStream.Position = 0;

                return memoryStream;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading file from SFTP server. Remote path: {RemotePath}", remotePath);
                throw;
            }
        }

        public bool FileExists(string remotePath)
        {
            var config = _configuration.GetSection("Upload:RemoteServer");
            if (!config.GetValue<bool>("Enabled"))
            {
                return false;
            }

            var keyFile = Path.Combine(_environment.ContentRootPath, config["KeyFile"]);
            
            using var client = new SftpClient(new ConnectionInfo(
                config["Host"],
                config.GetValue<int>("Port"),
                config["Username"],
                new PrivateKeyAuthenticationMethod(config["Username"], new PrivateKeyFile(keyFile))
            ));

            client.Connect();
            return client.Exists(remotePath);
        }
    }
} 