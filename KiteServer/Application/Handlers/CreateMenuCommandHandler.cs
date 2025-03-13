using Application.Commands;
using Domain.System;
using Infrastructure.Exceptions;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.Handlers
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
                var db = context.Menus.Context;
                await ValidateMenuAsync(db, request);

                var menu = _mapper.Map<Menu>(request);
                menu.CreateBy = _currentUser.LoginName ?? "system";
                menu.UpdateBy = _currentUser.LoginName ?? "system";
                
                await context.Menus.InsertAsync(menu);
                context.Commit();

                return true;
            }
        }

        private async Task ValidateMenuAsync(ISqlSugarClient db, CreateMenuCommand request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("菜单名称不能为空");

            if (request.ParentId.HasValue && request.ParentId.Value != 0)
            {
                var parent = await db.Queryable<Menu>()
                    .FirstAsync(x => x.Id == request.ParentId.Value);

                if (parent == null)
                    throw new UserFriendlyException("父菜单不存在");
            }

            var existingMenu = await db.Queryable<Menu>()
                .FirstAsync(x => x.Name == request.Name && x.ParentId == request.ParentId);

            if (existingMenu != null)
                throw new UserFriendlyException("同级菜单下已存在相同名称的菜单");
        }
    }
} 