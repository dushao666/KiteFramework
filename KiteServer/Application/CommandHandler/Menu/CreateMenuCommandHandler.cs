using Application.Command.Menu;
using Infrastructure.Exceptions;
using Infrastructure.Extension;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandler.Menu
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICurrentUser _currentUser;

        public CreateMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, IMapper mapper, ICurrentUser currentUser)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _currentUser = currentUser;
        }

        public async Task<bool> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                await ValidateMenuAsync(context, request);

                var menu = _mapper.Map<Domain.System.Menu>(request);
                
                // 只设置创建者和更新者，CreateTime 和 UpdateTime 由 SqlSugar AOP 自动设置
                menu.CreateBy = _currentUser.LoginName ?? "system";
                menu.UpdateBy = _currentUser.LoginName ?? "system";
                
                await context.Menus.InsertAsync(menu);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(DBContext context, CreateMenuCommand request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("菜单名称不能为空");

            if (request.ParentId.HasValue && request.ParentId.Value != 0)
            {
                var parent = await context.Menus
                    .GetFirstAsync(x => x.Id == request.ParentId.Value);

                if (parent == null)
                    throw new UserFriendlyException("父菜单不存在");
            }

            var existingMenu = await context.Menus
                .GetFirstAsync(x => x.Name == request.Name && x.ParentId == request.ParentId);

            if (existingMenu != null)
                throw new UserFriendlyException("同级菜单下已存在相同名称的菜单");
        }
    }
}