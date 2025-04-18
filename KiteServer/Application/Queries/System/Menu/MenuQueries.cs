using DomainShared.Dto.System;
using MapsterMapper;
using Repository.Repositories;

namespace Application.Queries.System.Menu
{
    /// <summary>
    /// 菜单查询实现
    /// </summary>
    public class MenuQueries : IMenuQueries
    {
        private readonly IServiceProvider _serviceProvider;
        
        public MenuQueries(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// 获取菜单树
        /// </summary>
        public async Task<List<MenuDto>> GetMenuTreeAsync(bool includeHidden = false)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                
                using (var context = unitOfWork.CreateContext())
                {
                    // 获取菜单数据
                    var db = context.Menus.Context;
                    var menus = await db.Queryable<Domain.System.Menu>()
                        .Where(x => !x.IsDeleted)
                        .WhereIF(!includeHidden, x => !x.IsHidden)
                        .OrderBy(x => x.Sort)
                        .ToListAsync();

                    // 转换为DTO
                    var menuDtos = mapper.Map<List<MenuDto>>(menus);

                    // 构建树形结构，从根节点（ParentId = 0）开始
                    return BuildMenuTree(menuDtos, 0);
                }
            }
        }

        /// <summary>
        /// 获取菜单列表
        /// </summary>
        public async Task<List<MenuDto>> GetMenuListAsync(string keyword = null, bool includeHidden = false)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                
                using (var context = unitOfWork.CreateContext())
                {
                    var db = context.Menus.Context;
                    
                    // 首先获取所有未删除的菜单
                    var allMenus = await db.Queryable<Domain.System.Menu>()
                        .Where(x => !x.IsDeleted)
                        .WhereIF(!includeHidden, x => !x.IsHidden)
                        .OrderBy(x => x.Sort)
                        .ToListAsync();
                    
                    // 转换为DTO
                    var allMenuDtos = mapper.Map<List<MenuDto>>(allMenus);
                    
                    // 如果有关键字搜索
                    if (!string.IsNullOrEmpty(keyword))
                    {
                        // 找出匹配关键字的菜单ID
                        var matchedMenuIds = new HashSet<long>(
                            allMenuDtos.Where(m => m.Name.Contains(keyword))
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
        }

        /// <summary>
        /// 获取当前用户的菜单 
        /// </summary>
        public async Task<List<MenuDto>> GetUserMenusAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<ISugarUnitOfWork<DbContext>>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var currentUser = scope.ServiceProvider.GetRequiredService<Infrastructure.Extension.ICurrentUser>();
                
                using (var context = unitOfWork.CreateContext())
                {
                    var db = context.Menus.Context;
                    
                    // 管理员用户，返回所有菜单
                    bool returnAllMenus =  (currentUser.IsAuthenticated && currentUser.LoginName == "admin");
                    
                    if (returnAllMenus)
                    {
                        // 获取所有菜单
                        var allMenus = await db.Queryable<Domain.System.Menu>()
                            .Where(m => !m.IsDeleted && !m.IsHidden)
                            .OrderBy(m => m.Sort)
                            .ToListAsync();

                        // 转换为DTO
                        var allMenuDtos = mapper.Map<List<MenuDto>>(allMenus);
                        
                        // 构建树形结构
                        var menuTree = BuildMenuTree(allMenuDtos, 0);
                        
                        // 处理所有DTO，确保id是字符串类型而不是数字类型
                        // 前端期望菜单的ID是字符串类型
                        ProcessMenuIds(menuTree);
                        
                        // 构建完整路径
                        BuildFullPaths(menuTree, "");
                        
                        return menuTree;
                    }
                    
                    // 正常权限检查流程
                    // 如果用户未登录，返回空列表
                    if (!currentUser.IsAuthenticated || !currentUser.UserId.HasValue)
                    {
                        return new List<MenuDto>();
                    }
                    
                    // 获取用户ID
                    var userId = currentUser.UserId.Value;
                    
                    // 获取用户角色
                    var userRoles = await db.Queryable<Domain.System.UserRole>()
                        .Where(ur => ur.UserId == userId && !ur.IsDeleted)
                        .Select(ur => ur.RoleId)
                        .ToListAsync();
                    
                    // 如果用户没有角色，返回空列表
                    if (!userRoles.Any())
                    {
                        return new List<MenuDto>();
                    }
                    
                    // 获取角色菜单
                    var menuIds = await db.Queryable<Domain.System.RoleMenu>()
                        .Where(rm => userRoles.Contains(rm.RoleId) && !rm.IsDeleted)
                        .Select(rm => rm.MenuId)
                        .Distinct()
                        .ToListAsync();
                    
                    // 如果没有菜单权限，返回空列表
                    if (!menuIds.Any())
                    {
                        return new List<MenuDto>();
                    }
                    
                    // 获取菜单数据
                    var menus = await db.Queryable<Domain.System.Menu>()
                        .Where(m => menuIds.Contains(m.Id) && !m.IsDeleted && !m.IsHidden)
                        .OrderBy(m => m.Sort)
                        .ToListAsync();
                    
                    // 转换为DTO
                    var menuDtos = mapper.Map<List<MenuDto>>(menus);
                    
                    // 构建树形结构
                    var result = BuildMenuTree(menuDtos, 0);
                    
                    // 处理所有DTO，确保id是字符串类型而不是数字类型
                    ProcessMenuIds(result);
                    
                    // 构建完整路径
                    BuildFullPaths(result, "");
                    
                    return result;
                }
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

        /// <summary>
        /// 将菜单ID转换为字符串格式
        /// </summary>
        private void ProcessMenuIds(List<MenuDto> menus)
        {
            foreach (var menu in menus)
            {
                // ID已经是字符串类型，所以不需要转换
                // 但如果需要格式化，可以在这里进行
                
                // 处理子菜单
                if (menu.Children != null && menu.Children.Any())
                {
                    ProcessMenuIds(menu.Children);
                }
            }
        }

        /// <summary>
        /// 构建完整的路径，确保子路由的path不包含父路由前缀
        /// </summary>
        private void BuildFullPaths(List<MenuDto> menus, string parentPath)
        {
            foreach (var menu in menus)
            {
                // 处理路径，确保子路由的path不包含父路由前缀
                if (!string.IsNullOrEmpty(menu.Path))
                {
                    // 确保路径不以/开头
                    if (menu.Path.StartsWith("/"))
                    {
                        menu.Path = menu.Path.Substring(1);
                    }

                    // 如果它是顶级菜单且不是以/开头，则添加前导/
                    if (string.IsNullOrEmpty(parentPath) && !menu.Path.StartsWith("/"))
                    {
                        menu.Path = "/" + menu.Path;
                    }
                }
                
                // 处理子菜单
                if (menu.Children != null && menu.Children.Any())
                {
                    // 传递当前菜单的完整路径给子菜单
                    BuildFullPaths(menu.Children, menu.Path);
                }
            }
        }
    }
} 