using Application.Command.FileAndFolder;
using Domain.FileInfo;
using Repository.Repositories;

namespace Application.CommandHandler.FileAndFolder
{
    public class DeleteFileCommandHandler : IRequestHandler<DeleteFileCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteFileCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteFileCommand command, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查文件是否存在，并且属于当前用户
                var file = await context.Files.GetFirstAsync(x =>
                    x.UserId == _currentUser.UserId &&
                    x.Id == command.FileId);

                if (file == null)
                {
                    throw new UserFriendlyException("文件不存在或无权删除");
                }

                // 逻辑删除：标记文件为已删除
                file.IsDeleted = true;

                // 更新数据库
                await context.Files.UpdateAsync(file);

                // 提交更改
                context.Commit();
            }

            return true;
        }
    }
}