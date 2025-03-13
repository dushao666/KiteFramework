using Application.Command.Menu;
using Domain.System;
using Repository.Repositories;

namespace Application.Handlers
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;

        public DeleteMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var db = context.Menus.Context;
                await ValidateMenuAsync(db, request.Id);

                await context.Menus.DeleteByIdAsync(request.Id);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(ISqlSugarClient db, long menuId)
        {
            var menu = await db.Queryable<Menu>()
                .FirstAsync(x => x.Id == menuId)
                ?? throw new UserFriendlyException($"未找到ID为{menuId}的菜单");

            var hasChildren = await db.Queryable<Menu>()
                .AnyAsync(x => x.ParentId == menuId);

            if (hasChildren)
                throw new UserFriendlyException("该菜单下存在子菜单，无法删除");
        }
    }
} 