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
                    var roles = new List<Role>
                    {
                        new Role
                        {
                            Name = "超级管理员",
                            Code = "admin",
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Description = "超级管理员，拥有所有权限"
                        },
                        new Role
                        {
                            Name = "普通用户",
                            Code = "user",
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                            Description = "普通用户，拥有基本权限"
                        }
                    };

                    // 逐个插入角色并获取ID
                    foreach (var role in roles)
                    {
                        var roleId = context.Roles.Context.Insertable(role).ExecuteReturnBigIdentity();
                        role.Id = roleId;
                        Console.WriteLine($"创建角色: {role.Name}, ID: {role.Id}");
                    }
                    
                    context.Commit();

                    // 获取管理员角色
                    var adminRole = roles.First(r => r.Code == "admin");

                    // 获取所有菜单
                    var allMenus = context.Menus.GetList(m => !m.IsDeleted);
                    if (allMenus.Any())
                    {
                        // 为管理员角色分配所有菜单权限
                        var roleMenus = new List<RoleMenu>();
                        foreach (var menu in allMenus)
                        {
                            roleMenus.Add(new RoleMenu
                            {
                                RoleId = adminRole.Id,
                                MenuId = menu.Id,
                                CreateBy = "system",
                                UpdateBy = "system"
                            });
                        }

                        // 批量插入角色菜单关联
                        if (roleMenus.Any())
                        {
                            context.RoleMenus.InsertRange(roleMenus);
                            context.Commit();
                            Console.WriteLine($"为角色 {adminRole.Name} (ID: {adminRole.Id}) 分配了 {roleMenus.Count} 个菜单权限");
                        }
                    }
                    else
                    {
                        Console.WriteLine("没有找到菜单数据，跳过角色菜单关联");
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