using Domain.System;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class UserSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

        public UserSeedData(ISugarUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查用户表是否有数据
                var userCount = context.Users.AsQueryable().Count();
                if (userCount > 0)
                {
                    return;
                }

                try
                {
                    // 创建默认管理员用户
                    var adminUser = new User
                    {
                        Name = "admin",
                        NickName = "admin",
                        PassWord = "123456", // 实际应用中应该加密存储
                        DingUserId = "666666",
                        Status = "0",
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    // 插入用户数据
                    context.Users.Insert(adminUser);
                    context.Commit();
                    
                    // 获取管理员角色
                    var adminRole = context.Roles.GetFirst(r => r.Code == "admin");
                    
                    // 如果没有管理员角色，创建一个
                    if (adminRole == null)
                    {
                        adminRole = new Role
                        {
                            Name = "超级管理员",
                            Code = "admin",
                            Status = 0,
                            CreateBy = "system",
                            UpdateBy = "system",
                        };
                        
                        context.Roles.Insert(adminRole);
                        context.Commit();
                    }
                    
                    // 为管理员用户分配管理员角色
                    var userRole = new UserRole
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };
                    
                    // 插入用户角色关联
                    context.UserRoles.Insert(userRole);
                    context.Commit();
                    
                    // 获取所有菜单
                    var allMenus = context.Menus.GetList(m => !m.IsDeleted);
                    
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
                    }
                    
                    Console.WriteLine("用户种子数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"用户种子数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
}