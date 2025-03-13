using Application.Dtos;
using Application.Queries.Menu;
using Domain.System;
using MapsterMapper;
using MediatR;
using Repository.Repositories;
using SqlSugar;

namespace Application.Handlers
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
                var db = context.Menus.Context;
                var query = db.Queryable<Menu>();
                
                if (!request.IncludeHidden)
                    query = query.Where(x => !x.IsHidden);

                var menus = await query
                    .OrderBy(x => x.Sort)
                    .ToListAsync();

                var menuDtos = _mapper.Map<List<MenuDto>>(menus);
                return BuildMenuTree(menuDtos, 0);
            }
        }

        private List<MenuDto> BuildMenuTree(List<MenuDto> menus, long? parentId = null)
        {
            return menus
                .Where(m => m.ParentId == parentId)
                .Select(m => {
                    m.Children = BuildMenuTree(menus, m.Id);
                    return m;
                })
                .OrderBy(m => m.Sort)
                .ToList();
        }
    }
} 