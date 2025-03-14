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

                    // 插入用户数据并获取ID
                    var userId = context.Users.Context.Insertable(adminUser).ExecuteReturnBigIdentity();
                    adminUser.Id = userId;
                    context.Commit();

                    Console.WriteLine($"创建的管理员用户ID: {adminUser.Id}");

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

                        // 插入角色数据并获取ID
                        var roleId = context.Roles.Context.Insertable(adminRole).ExecuteReturnBigIdentity();
                        adminRole.Id = roleId;
                        context.Commit();
                    }

                    Console.WriteLine($"管理员角色ID: {adminRole.Id}");

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

                    Console.WriteLine($"创建用户角色关联: 用户ID={adminUser.Id}, 角色ID={adminRole.Id}");

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
                        Console.WriteLine($"为角色ID={adminRole.Id}分配了{roleMenus.Count}个菜单权限");
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