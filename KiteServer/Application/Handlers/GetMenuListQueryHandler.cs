using Application.Dtos;
using Application.Queries.Menu;
using Domain.System;
using MapsterMapper;

namespace Application.Handlers
{
    public class GetMenuListQueryHandler : IRequestHandler<GetMenuListQuery, List<MenuDto>>
    {
        private readonly ISqlSugarClient _db;
        private readonly IMapper _mapper;

        public GetMenuListQueryHandler(ISqlSugarClient db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuListQuery request, CancellationToken cancellationToken)
        {
            var query = _db.Queryable<Menu>();

            if (!request.IncludeHidden)
                query = query.Where(x => !x.IsHidden);

            if (!string.IsNullOrEmpty(request.Keyword))
                query = query.Where(x => x.Name.Contains(request.Keyword));

            var menus = await query
                .OrderBy(x => x.Sort)
                .ToListAsync();

            return _mapper.Map<List<MenuDto>>(menus);
        }
    }
} 