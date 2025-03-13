using Application.Commands.Menu;
using Repository.Repositories;

namespace Application.CommandHandlers.Menu
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public DeleteMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 验证菜单是否存在及是否有子菜单
                await ValidateMenuAsync(context, request.Id);

                // 获取要删除的菜单
                var menu = await context.Menus.GetFirstAsync(x => x.Id == request.Id);
                
                // 更新菜单的更新者信息
                menu.UpdateBy = _currentUser.LoginName ?? "system";
                
                // 使用 DbSet 的 DeleteAsync 方法进行逻辑删除
                // 这会将 IsDeleted 设置为 true 而不是从数据库中删除记录
                var result = await context.Menus.DeleteAsync(menu);
                
                // 提交事务
                context.Commit();

                return result;
            }
        }

        private async Task ValidateMenuAsync(DBContext context, long menuId)
        {
            var menu = await context.Menus
                .GetFirstAsync(x => x.Id == menuId && !x.IsDeleted)
                ?? throw new UserFriendlyException($"未找到ID为{menuId}的菜单");

            var childMenu = await context.Menus
                .GetFirstAsync(x => x.ParentId == menuId && !x.IsDeleted);

            if (childMenu != null)
                throw new UserFriendlyException("该菜单下存在子菜单，无法删除");
        }
    }
} 