using Application.Command.Menu;
using Infrastructure.Exceptions;
using MediatR;
using Repository.Repositories;
using SqlSugar;

namespace Application.CommandHandler.Menu
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
                await ValidateMenuAsync(context, request.Id);

                await context.Menus.DeleteByIdAsync(request.Id);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(DBContext context, long menuId)
        {
            var menu = await context.Menus
                .GetFirstAsync(x => x.Id == menuId)
                ?? throw new UserFriendlyException($"未找到ID为{menuId}的菜单");

            var childMenu = await context.Menus
                .GetFirstAsync(x => x.ParentId == menuId);

            if (childMenu != null)
                throw new UserFriendlyException("该菜单下存在子菜单，无法删除");
        }
    }
} 