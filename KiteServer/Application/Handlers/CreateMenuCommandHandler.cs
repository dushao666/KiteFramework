using Application.Commands;

using Domain.System;
using Infrastructure.Exceptions;
using MapsterMapper;
using MediatR;
using SqlSugar;

namespace Application.Handlers
{
    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, bool>
    {
        private readonly ISqlSugarClient _db;
        private readonly IMapper _mapper;

        public CreateMenuCommandHandler(ISqlSugarClient db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<bool> Handle(CreateMenuCommand request, CancellationToken cancellationToken)
        {
            await ValidateMenuAsync(request);
            var menu = _mapper.Map<Menu>(request);
            return await _db.Insertable(menu).ExecuteCommandAsync() > 0;
        }

        private async Task ValidateMenuAsync(CreateMenuCommand request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new UserFriendlyException("菜单名称不能为空");

            if (request.ParentId.HasValue)
            {
                var parentExists = await _db.Queryable<Menu>()
                    .AnyAsync(x => x.Id == request.ParentId.Value);

                if (!parentExists)
                    throw new UserFriendlyException("父菜单不存在");
            }

            var nameExists = await _db.Queryable<Menu>()
                .AnyAsync(x => x.Name == request.Name && x.ParentId == request.ParentId);

            if (nameExists)
                throw new UserFriendlyException("同级菜单下已存在相同名称的菜单");
        }
    }
} 