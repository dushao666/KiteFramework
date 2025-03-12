using Application.Commands;
using Infrastructure.Exceptions;
using MediatR;
using SqlSugar;

namespace Application.Handlers
{
    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly ISqlSugarClient _db;

        public DeleteMenuCommandHandler(ISqlSugarClient db)
        {
            _db = db;
        }

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken cancellationToken)
        {
            await ValidateMenuAsync(request.Id);
            return await _db.Deleteable<Domain.System.Menu>()
                .Where(x => x.Id == request.Id)
                .ExecuteCommandAsync() > 0;
        }

        private async Task ValidateMenuAsync(long menuId)
        {
            var menu = await _db.Queryable<Domain.System.Menu>()
                .FirstAsync(x => x.Id == menuId)
                ?? throw new UserFriendlyException($"未找到ID为{menuId}的菜单");

            var hasChildren = await _db.Queryable<Domain.System.Menu>()
                .AnyAsync(x => x.ParentId == menuId);

            if (hasChildren)
                throw new UserFriendlyException("该菜单下存在子菜单，无法删除");
        }
    }
} 