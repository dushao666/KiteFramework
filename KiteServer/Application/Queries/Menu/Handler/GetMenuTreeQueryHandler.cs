using Application.Dtos;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

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
                var query = context.Menus.AsQueryable();
                
                if (!request.IncludeHidden)
                    query = query.Where(x => !x.IsHidden);

                var menus = await query
                    .OrderBy(x => x.Sort)
                    .ToListAsync();

                var menuDtos = _mapper.Map<List<MenuDto>>(menus);
                return BuildMenuTree(menuDtos);
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