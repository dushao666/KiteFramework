using Application.Dtos;
using Application.Queries.Menu;
using Domain.System;
using MapsterMapper;

namespace Application.Handlers
{
    public class GetMenuTreeQueryHandler : IRequestHandler<GetMenuTreeQuery, List<MenuDto>>
    {
        private readonly ISqlSugarClient _db;
        private readonly IMapper _mapper;

        public GetMenuTreeQueryHandler(ISqlSugarClient db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuTreeQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<Menu>();
            
            if (!request.IncludeHidden)
                query = query.Where(x => !x.IsHidden);

            var menus = await query
                .OrderBy(x => x.Sort)
                .ToListAsync();

            var menuDtos = _mapper.Map<List<MenuDto>>(menus);
            return BuildMenuTree(menuDtos);
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