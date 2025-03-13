using Application.Command.FileAndFolder;
using Domain.FileAndFolderInfo;
using Repository.Repositories;

namespace Application.CommandHandler.FileAndFolder
{
    /// <summary>
    /// 保存文件夹命令处理器
    /// </summary>
    public class SaveFolderCommandHandler : IRequestHandler<SaveFolderCommand, bool>
    {
        private readonly ICurrentUser _currentUser;
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;

        public SaveFolderCommandHandler(ICurrentUser currentUser, ISugarUnitOfWork<DBContext> unitOfWork)
        {
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }


        public async Task<bool> Handle(SaveFolderCommand command, CancellationToken cancellationToken)
        {
            // 检查是否是新文件夹（Id 为空表示新增）
            if (command.Id == null)
            {
                using (var context = _unitOfWork.CreateContext())
                {
                    // 检查是否已存在同名文件夹
                    var exist = await context.Folders.IsAnyAsync(x =>
                        x.UserId == _currentUser.UserId &&
                        x.Name == command.Name &&
                        x.ParentId == command.ParentId);

                    if (!exist)
                    {
                        // 创建新文件夹
                        FolderInfos infos = new FolderInfos(_currentUser.UserId.Value, command.Name, command.ParentId,
                            command.Id);
                        await context.Folders.InsertAsync(infos);
                        context.Commit();
                    }
                    else
                    {
                        // 文件夹已存在
                        return false;
                    }
                }
            }
            else
            {
                // 更新现有文件夹
                using (var context = _unitOfWork.CreateContext())
                {
                    // 获取要更新的文件夹
                    var folder = await context.Folders.GetFirstAsync(x =>
                        x.Id == command.Id && x.UserId == _currentUser.UserId);

                    if (folder != null)
                    {
                        // 检查同一父目录下是否存在同名文件夹
                        var existingFolder = await context.Folders.GetFirstAsync(x =>
                            x.Name == command.Name && x.ParentId == command.ParentId &&
                            x.UserId == _currentUser.UserId && x.Id != command.Id);

                        if (existingFolder != null)
                        {
                            // 存在同名文件夹，返回 false
                            return false;
                        }

                        // 更新文件夹信息
                        folder.Name = command.Name;
                        folder.ParentId = command.ParentId;
                        await context.Folders.UpdateAsync(folder);
                        context.Commit();

                        // 更新成功，返回 true
                        return true;
                    }
                    else
                    {
                        // 文件夹不存在，返回 false
                        return false;
                    }
                }
            }

            return true;
        }
    }
}