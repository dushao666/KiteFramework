using Application.Command.Menu;
using Infrastructure.Exceptions;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.CommandHandler.Menu
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, bool>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IMapper _mapper;

        public CreateMenuCommandHandler(ISugarUnitOfWork<DBContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                await ValidateMenuAsync(context, request);

                var menu = _mapper.Map<Domain.System.Menu>(request);
                await context.Menus.InsertAsync(menu);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(DBContext context, CreateMenuCommand request)
        {
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
                .GetFirstAsync(x => x.Name == request.Name && x.ParentId == request.ParentId);

            if (existingMenu != null)
                throw new UserFriendlyException("同级菜单下已存在相同名称的菜单");
        }
    }
} 