using Application.Dtos;
using Domain.System;
using MapsterMapper;
using MediatR;
using Repository.Repositories;
using SqlSugar;

namespace Application.Queries.Menu.Handler
{
    public class GetMenuTreeQueryHandler : IRequestHandler<GetMenuTreeQuery, List<MenuDto>>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IMapper _mapper;

        public GetMenuTreeQueryHandler(ISugarUnitOfWork<DBContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 获取菜单数据
                var db = context.Menus.Context;
                var menus = await db.Queryable<Domain.System.Menu>()
                    .WhereIF(!request.IncludeHidden, x => !x.IsHidden)
                    .OrderBy(x => x.Sort)
                    .ToListAsync();

                // 转换为DTO
                var menuDtos = _mapper.Map<List<MenuDto>>(menus);

                // 构建树形结构，从根节点（ParentId = 0）开始
                return BuildMenuTree(menuDtos, 0);
            }
        }

        private List<MenuDto> BuildMenuTree(List<MenuDto> menus, long parentId)
        {
            var children = menus
                .Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Sort)
                .ToList();

            foreach (var child in children)
            {
                child.Children = BuildMenuTree(menus, child.Id);
            }

            return children;
        }
    }
}