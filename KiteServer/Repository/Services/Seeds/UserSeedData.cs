using Domain.System;
using Microsoft.Extensions.Configuration;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class UserSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly IConfiguration _configuration;

        public UserSeedData(ISugarUnitOfWork<DbContext> unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查是否需要重新创建数据库
                bool recreateDatabase = _configuration.GetValue<bool>("SeedData:RecreateDatabase", false);
                
                // 如果不是重新创建数据库，则检查表中是否已有数据
                if (!recreateDatabase)
                {
                    // 检查用户表是否有数据
                    var userCount = context.Users.AsQueryable().Count();
                    if (userCount > 0)
                    {
                        Console.WriteLine("用户数据已存在，跳过初始化");
                        return;
                    }
                }

                try
                {
                    Console.WriteLine("开始初始化用户数据...");
                    
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
                    var adminRole = context.Roles.GetFirst(r => r.Code == "admin" && !r.IsDeleted);
                    if (adminRole == null)
                    {
                        Console.WriteLine("未找到管理员角色，跳过用户角色关联");
                        return;
                    }
                    
                    Console.WriteLine($"找到管理员角色ID: {adminRole.Id}");

                    // 检查是否已存在用户角色关联
                    var existingUserRole = context.UserRoles.GetFirst(ur => ur.UserId == adminUser.Id && ur.RoleId == adminRole.Id);
                    if (existingUserRole != null)
                    {
                        Console.WriteLine($"用户 {adminUser.Name} 已关联角色 {adminRole.Name}，跳过关联");
                    }
                    else
                    {
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
                    }

                    // 检查角色菜单关联是否已存在
                    var existingRoleMenuCount = context.RoleMenus.GetList(rm => rm.RoleId == adminRole.Id).Count;
                    if (existingRoleMenuCount > 0)
                    {
                        Console.WriteLine($"角色 {adminRole.Name} 已有 {existingRoleMenuCount} 个菜单关联，跳过分配");
                        Console.WriteLine("用户种子数据初始化成功");
                        return;
                    }

                    // 获取所有菜单
                    var allMenus = context.Menus.GetList(m => !m.IsDeleted);
                    if (!allMenus.Any())
                    {
                        Console.WriteLine("没有找到菜单数据，跳过角色菜单关联");
                        Console.WriteLine("用户种子数据初始化成功");
                        return;
                    }

                    Console.WriteLine($"找到 {allMenus.Count} 个菜单");

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
                    Console.WriteLine(ex.StackTrace);
                    throw;
                }
            }
        }
    }
}