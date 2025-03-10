using DomainShared.Dto.FileAndFolderInfo;
using Repository.Repositories;

namespace Application.Queries
{
    /// <summary>
    /// 文件夹查询器
    /// </summary>
    public class FolderQueries : IFolderQueries
    {
        private readonly IServiceProvider _serviceProvider;
        private ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public FolderQueries(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = _serviceProvider.GetService<ISugarUnitOfWork<DBContext>>();
            _currentUser = _serviceProvider.GetService<ICurrentUser>();
        }

        /// <summary>
        /// 根据用户ID获取文件夹树
        /// </summary>
        /// <summary>
        public async Task<List<FolderDto>> GetFolderTreeAsync()
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                const long ROOT_FOLDER_ID = 0;

                // 查询属于当前用户的所有文件夹
                var folders = await context.Folders.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .Select(x => new FolderDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        UserId = x.UserId,
                        UpdateTime = x.UpdateTime
                    })
                    .ToListAsync();

                // 查询属于当前用户的所有文件
                var files = await context.Files.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .Select(x => new FileDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FileFullName = x.FileName,
                        Size = x.Size,
                        Path = x.Path,
                        FileType = x.FileType,
                        Md5Hash = x.Md5Hash,
                        FolderId = x.FolderId,
                        UpdateTime = x.UpdateTime
                    })
                    .ToListAsync();

                // 构建树形结构
                var result = BuildFolderTree(folders, files, null);

                // 添加根目录下的文件（FolderId 为 null 的文件）
                var rootFiles = files.Where(f => f.FolderId == null).ToList();
                if (rootFiles.Any())
                {
                    result.Add(new FolderDto
                    {
                        Id = ROOT_FOLDER_ID,
                        Name = "根目录",
                        ParentId = null,
                        UserId = _currentUser.UserId.Value,
                        UpdateTime = DateTime.Now,
                        Children = new List<FolderDto>(),
                        Files = rootFiles
                    });
                }

                return result;
            }
        }

        /// <summary>
        /// 获取单个文件夹详情
        /// </summary>
        public async Task<FolderDetailsDto> GetFolderDetailsAsync(long folderId)
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                var folder = await context.Folders.GetByIdAsync(folderId);
                if (folder == null) throw new UserFriendlyException("文件夹不存在");

                // 权限校验
                if (_currentUser.LoginName != "admin" && folder.UserId != _currentUser.UserId)
                {
                    throw new UserFriendlyException("无权访问此文件夹");
                }

                return folder.Adapt<FolderDetailsDto>();
            }
        }

        /// <summary>
        /// 递归构建文件夹树，并关联文件
        /// </summary>
        private List<FolderDto> BuildFolderTree(List<FolderDto> folders, List<FileDto> files, long? parentId)
        {
            return folders
                .Where(x => x.ParentId == parentId) // 找到当前层级的文件夹
                .Select(folder =>
                {
                    // 关联文件：找到属于当前文件夹的所有文件
                    folder.Files = files.Where(file => file.FolderId == folder.Id).ToList();

                    // 递归构建子文件夹
                    folder.Children = BuildFolderTree(folders, files, folder.Id);

                    return folder;
                })
                .ToList();
        }


        public async Task<List<FolderDto>> GetFolderTreeAsync(FolderSearchDto search)
        {
            using (var context = _unitOfWork.CreateContext(false))
            {
                // 1. 获取所有文件夹
                var allFolders = await context.Folders.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .Select(x => new FolderDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        ParentId = x.ParentId,
                        UserId = x.UserId,
                        UpdateTime = x.UpdateTime
                    })
                    .ToListAsync();

                // 2. 获取所有文件
                var allFiles = await context.Files.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .WhereIF(!string.IsNullOrWhiteSpace(search?.Keyword),
                        x => x.Name.Contains(search.Keyword) || x.FileName.Contains(search.Keyword))
                    .WhereIF(!string.IsNullOrWhiteSpace(search?.FileType),
                        x => x.FileType == search.FileType)
                    .Select(x => new FileDto
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FileFullName = x.FileName,
                        Size = x.Size,
                        Path = x.Path,
                        FileType = x.FileType,
                        Md5Hash = x.Md5Hash,
                        FolderId = x.FolderId,
                        UpdateTime = x.UpdateTime
                    })
                    .ToListAsync();

                // 3. 如果有搜索条件
                if (!string.IsNullOrWhiteSpace(search?.Keyword))
                {
                    // 找到名称匹配的文件夹
                    var matchedFolders = allFolders
                        .Where(f => f.Name.Contains(search.Keyword))
                        .ToList();

                    if (matchedFolders.Any())
                    {
                        // 只返回匹配的文件夹及其文件
                        var result = matchedFolders.Select(folder => new FolderDto
                        {
                            Id = folder.Id,
                            Name = folder.Name,
                            ParentId = folder.ParentId,
                            UserId = folder.UserId,
                            UpdateTime = folder.UpdateTime,
                            Children = GetChildFolders(folder.Id.ToString(), allFolders),
                            Files = allFiles.Where(f => f.FolderId == folder.Id).ToList()
                        }).ToList();

                        return result;
                    }
                    else
                    {
                        // 如果没有匹配的文件夹，检查是否有匹配的文件
                        var matchedFiles = allFiles.Where(f =>
                            f.Name.Contains(search.Keyword) ||
                            f.FileFullName.Contains(search.Keyword)).ToList();

                        if (matchedFiles.Any())
                        {
                            // 获取包含匹配文件的文件夹
                            var folderIds = matchedFiles.Select(f => f.FolderId).Distinct();
                            var foldersWithMatchedFiles = allFolders
                                .Where(f => folderIds.Contains(f.Id))
                                .Select(folder => new FolderDto
                                {
                                    Id = folder.Id,
                                    Name = folder.Name,
                                    ParentId = folder.ParentId,
                                    UserId = folder.UserId,
                                    UpdateTime = folder.UpdateTime,
                                    Children = GetChildFolders(folder.Id.ToString(), allFolders),
                                    Files = matchedFiles.Where(f => f.FolderId == folder.Id).ToList()
                                })
                                .ToList();

                            return foldersWithMatchedFiles;
                        }
                    }

                    // 如果没有匹配的文件夹和文件，返回空列表
                    return new List<FolderDto>();
                }

                // 4. 如果没有搜索条件，返回完整的树
                return BuildFolderTree(allFolders, allFiles, null);
            }
        }

        private List<FolderDto> GetChildFolders(string parentId, List<FolderDto> allFolders)
        {
            return allFolders
                .Where(f => f.ParentId.ToString() == parentId)
                .Select(child => new FolderDto
                {
                    Id = child.Id,
                    Name = child.Name,
                    ParentId = child.ParentId,
                    UserId = child.UserId,
                    UpdateTime = child.UpdateTime,
                    Children = GetChildFolders(child.Id.ToString(), allFolders),
                    Files = new List<FileDto>() // 子文件夹的文件列表可以为空，因为搜索结果主要关注匹配的文件夹
                })
                .ToList();
        }
    }
}