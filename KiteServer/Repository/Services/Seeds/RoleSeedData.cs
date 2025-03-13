using Domain.System;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class RoleSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

        public RoleSeedData(ISugarUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查角色表是否有数据
                var roleCount = context.Roles.AsQueryable().Count();
                if (roleCount > 0)
                {
                    return;
                }

                try
                {
                    // 创建默认角色
                    var adminRole = new Role
                    {
                        Name = "超级管理员",
                        Code = "admin",
                        Description = "系统超级管理员，拥有所有权限",
                        Status = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    var operatorRole = new Role
                    {
                        Name = "普通用户",
                        Code = "user",
                        Description = "普通用户，拥有基本权限",
                        Status = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    // 插入角色数据
                    context.Roles.Insert(adminRole);
                    context.Roles.Insert(operatorRole);
                    context.Commit();
                    
                    // 获取所有菜单ID
                    var menuIds = context.Menus.AsQueryable()
                        .Where(m => !m.IsDeleted)
                        .Select(m => m.Id)
                        .ToList();
                    
                    // 为管理员角色分配所有菜单权限
                    if (menuIds.Any())
                    {
                        var adminRoleMenus = menuIds.Select(menuId => new RoleMenu
                        {
                            RoleId = adminRole.Id,
                            MenuId = menuId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }).ToList();
                        
                        context.RoleMenus.InsertRange(adminRoleMenus);
                        context.Commit();
                    }
                    
                    Console.WriteLine("角色种子数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"角色种子数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
}