using Application.Dtos;
using Application.Queries.Menu;
using Domain.System;
using MapsterMapper;
using MediatR;
using Repository.Repositories;

namespace Application.Handlers
{
    public class GetMenuListQueryHandler : IRequestHandler<GetMenuListQuery, List<MenuDto>>
    {
        private readonly ISugarUnitOfWork<DBContext> _unitOfWork;
        private readonly IMapper _mapper;

        public GetMenuListQueryHandler(ISugarUnitOfWork<DBContext> unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<MenuDto>> Handle(GetMenuListQuery request, CancellationToken cancellationToken)
        {
            using (var context = _unitOfWork.CreateContext())
            {
                var db = context.Menus.Context;
                var query = db.Queryable<Menu>();

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
} 