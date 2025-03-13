using System.Security.Cryptography;
using Application.Command.FileAndFolder;
using Domain.FileInfo;
using Microsoft.AspNetCore.Hosting;
using Repository.Repositories;

namespace Application.CommandHandler.FileAndFolder
{
    /// <summary>
    /// 保存文件命令处理器
    /// </summary>
    public class SaveFileCommandHandler : IRequestHandler<SaveFileCommand, bool>
    {
        private readonly ICurrentUser _currentUser;
        private readonly IConfiguration _configuration;
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        
        public SaveFileCommandHandler(ICurrentUser currentUser, ISugarUnitOfWork<DBContext> unitOfWork,
            IConfiguration configuration,
            IWebHostEnvironment webHostEnvironment)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _configuration = configuration;
        }

        public async Task<bool> Handle(SaveFileCommand command, CancellationToken cancellationToken)
        {
            // 检查是否上传了文件
            if (command.File == null || command.File.Length == 0)
            {
                throw new ArgumentException("未上传有效的文件");
            }

            // 获取文件的基本信息
            var fileName = Path.GetFileName(command.File.FileName);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(command.File.FileName);
            var fileSize = command.File.Length;
            var fileType = command.File.ContentType;

            // 计算文件的 MD5 哈希值
            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = command.File.OpenReadStream())
                {
                    var hashBytes = md5.ComputeHash(stream);
                    md5Hash = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }

            // 构造文件存储路径（年/月/日）
            var now = DateTime.UtcNow;
            var year = now.Year.ToString();
            var month = now.Month.ToString("D2"); // 两位数格式
            var day = now.Day.ToString("D2"); // 两位数格式

            // 动态获取目标目录
            var targetDir = GetTargetDirectory("CommonFile", year, month, day);

            // 构造完整的文件路径
            var fullPath = Path.Combine(targetDir, fileName);

            // 保存文件到指定路径
            using (var fileStream = new FileStream(fullPath, FileMode.Create))
            {
                await command.File.CopyToAsync(fileStream, cancellationToken);
            }

            // 相对路径（用于存储到数据库）
            var relativePath = Path.Combine(year, month, day, fileName);

            // 检查是否是新文件（Id 为空表示新增）
            if (command.Id == null)
            {
                using (var context = _unitOfWork.CreateContext())
                {
                    // 检查是否已存在同名文件（在同一个文件夹下）
                    var exist = await context.Files.IsAnyAsync(x =>
                        x.UserId == _currentUser.UserId &&
                        x.Name == fileNameWithoutExtension &&
                        x.FolderId == command.FolderId);

                    if (!exist)
                    {
                        // 创建新文件记录
                        FileInfos fileInfo = new FileInfos(
                            _currentUser.UserId.Value,
                            fileNameWithoutExtension,
                            fileName,
                            command.FolderId,
                            fileSize,
                            relativePath, // 存储相对路径
                            fileType,
                            md5Hash);

                        await context.Files.InsertAsync(fileInfo);
                        context.Commit();
                    }
                    else
                    {
                        // 文件已存在
                        return false;
                    }
                }
            }
            else
            {
                // 更新现有文件
                using (var context = _unitOfWork.CreateContext())
                {
                    var file = await context.Files.GetFirstAsync(x =>
                        x.Id == command.Id && x.UserId == _currentUser.UserId);

                    if (file != null)
                    {
                        file.Name = fileNameWithoutExtension;
                        file.FileName = fileName;
                        file.FolderId = command.FolderId;
                        file.Size = fileSize;
                        file.Path = relativePath; // 更新为新的相对路径
                        file.FileType = fileType;
                        file.Md5Hash = md5Hash;

                        await context.Files.UpdateAsync(file);
                        context.Commit();
                    }
                }
            }

            return true;
        }
        
        
        private string GetTargetDirectory(string folderType, string year = null, string month = null, string day = null)
        {
            // 从配置文件中获取基础路径
            var basePath = _configuration[$"Upload:{folderType}"];

            // 如果是相对路径，则基于 WebRootPath 构造完整路径
            if (!Path.IsPathRooted(basePath))
            {
                basePath = Path.Combine(_webHostEnvironment.WebRootPath, basePath);
            }

            // 构造完整的目录路径
            var targetDir = Path.Combine(basePath, year ?? "", month ?? "", day ?? "");

            // 确保目录存在
            Directory.CreateDirectory(targetDir);

            return targetDir;
        }
    }
}