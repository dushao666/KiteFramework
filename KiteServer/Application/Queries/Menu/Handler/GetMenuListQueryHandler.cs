using DomainShared.Dto.System;
using MapsterMapper;
using Repository.Repositories;

namespace Application.Queries.Menu.Handler
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
                
                // 首先获取所有未删除的菜单
                var allMenus = await db.Queryable<Domain.System.Menu>()
                    .Where(x => !x.IsDeleted)
                    .WhereIF(!request.IncludeHidden, x => !x.IsHidden)
                    .OrderBy(x => x.Sort)
                    .ToListAsync();
                
                // 转换为DTO
                var allMenuDtos = _mapper.Map<List<MenuDto>>(allMenus);
                
                // 如果有关键字搜索
                if (!string.IsNullOrEmpty(request.Keyword))
                {
                    // 找出匹配关键字的菜单ID
                    var matchedMenuIds = new HashSet<long>(
                        allMenuDtos.Where(m => m.Name.Contains(request.Keyword))
                                  .Select(m => m.Id)
                    );
                    
                    // 找出所有匹配菜单的父菜单ID（向上查找）
                    var parentIds = new HashSet<long>();
                    foreach (var menu in allMenuDtos.Where(m => matchedMenuIds.Contains(m.Id)))
                    {
                        // 添加当前菜单的所有父菜单
                        var parentId = menu.ParentId;
                        while (parentId.HasValue && parentId.Value != 0)
                        {
                            parentIds.Add(parentId.Value);
                            var parent = allMenuDtos.FirstOrDefault(m => m.Id == parentId.Value);
                            if (parent == null) break;
                            parentId = parent.ParentId;
                        }
                    }
                    
                    // 找出所有匹配菜单的子菜单ID（向下查找）
                    var childIds = new HashSet<long>();
                    foreach (var id in matchedMenuIds)
                    {
                        AddChildMenuIds(allMenuDtos, id, childIds);
                    }
                    
                    // 合并所有需要显示的菜单ID
                    var displayMenuIds = new HashSet<long>(matchedMenuIds);
                    displayMenuIds.UnionWith(parentIds);
                    displayMenuIds.UnionWith(childIds);
                    
                    // 过滤菜单列表，只保留需要显示的菜单
                    allMenuDtos = allMenuDtos.Where(m => displayMenuIds.Contains(m.Id)).ToList();
                }
                
                // 构建树形结构
                return BuildMenuTree(allMenuDtos, 0);
            }
        }
        
        private void AddChildMenuIds(List<MenuDto> allMenus, long parentId, HashSet<long> childIds)
        {
            foreach (var menu in allMenus.Where(m => m.ParentId == parentId))
            {
                childIds.Add(menu.Id);
                AddChildMenuIds(allMenus, menu.Id, childIds);
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