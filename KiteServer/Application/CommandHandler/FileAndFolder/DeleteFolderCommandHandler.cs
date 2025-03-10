using Application.Command.FileAndFolder;
using Domain.FileAndFolderInfo;
using Repository.Repositories;


namespace Application.CommandHandler.FileAndFolder
{
    /// <summary>
    /// 删除文件夹命令处理器
    /// </summary>
    public class DeleteFolderCommandHandler : IRequestHandler<DeleteFolderCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteFolderCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }
        public async Task<bool> Handle(DeleteFolderCommand command, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 获取当前用户的所有文件夹和文件
                var folders = await context.Folders.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .ToListAsync();

                var files = await context.Files.AsQueryable()
                    .Where(x => x.UserId == _currentUser.UserId)
                    .ToListAsync();

                // 执行级联逻辑删除
                var folderIdsToDelete = GetFolderIdsToDelete(folders, command.FolderId).ToList();

                // 标记文件夹为已删除
                foreach (var folderId in folderIdsToDelete)
                {
                    var folder = folders.FirstOrDefault(x => x.Id == folderId);
                    if (folder != null)
                    {
                        folder.IsDeleted = true; // 逻辑删除
                        await context.Folders.UpdateAsync(folder);
                    }
                }

                // 标记文件为已删除
                var fileIdsToDelete = files.Where(x => folderIdsToDelete.Contains(x.FolderId.Value)).ToList();
                foreach (var file in fileIdsToDelete)
                {
                    file.IsDeleted = true; // 逻辑删除
                    await context.Files.UpdateAsync(file);
                }

                // 提交更改
                context.Commit();
            }

            return true;
        }


        /// <summary>
        /// 递归获取需要删除的文件夹ID列表
        /// </summary>
        private IEnumerable<long> GetFolderIdsToDelete(List<FolderInfos> folders, long folderId)
        {
            var folderIds = new List<long>(); // 初始化结果列表

            // 添加当前文件夹ID
            folderIds.Add(folderId);

            // 查找所有子文件夹
            var children = folders.Where(x => x.ParentId == folderId).ToList();
            foreach (var child in children)
            {
                folderIds.AddRange(GetFolderIdsToDelete(folders, child.Id)); // 递归处理子文件夹
            }

            return folderIds;
        }

    }
}