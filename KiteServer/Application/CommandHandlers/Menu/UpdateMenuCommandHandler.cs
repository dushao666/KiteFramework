using Application.Command.Menu;
using MapsterMapper;
using Repository.Repositories;

namespace Application.CommandHandler.Menu
{
    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                await ValidateMenuAsync(context, request);

                var menu = await context.Menus
                    .GetFirstAsync(x => x.Id == request.Id);

                _mapper.Map(request, menu);
                await context.Menus.UpdateAsync(menu);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(DBContext context, UpdateMenuCommand request)
        {
            var menu = await context.Menus
                .GetFirstAsync(x => x.Id == request.Id)
                ?? throw new UserFriendlyException($"未找到ID为{request.Id}的菜单");

            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("菜单名称不能为空");

            if (request.ParentId.HasValue)
            {
                var parent = await context.Menus
                    .GetFirstAsync(x => x.Id == request.ParentId.Value);

                if (parent == null)
                    throw new UserFriendlyException("父菜单不存在");
            }

            var existingMenu = await context.Menus
                .GetFirstAsync(x => x.Id != request.Id && x.Name == request.Name && x.ParentId == request.ParentId);

            if (existingMenu != null)
                throw new UserFriendlyException("同级菜单下已存在相同名称的菜单");
        }
    }
} 