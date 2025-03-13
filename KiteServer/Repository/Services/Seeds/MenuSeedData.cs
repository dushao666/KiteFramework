using Domain.System;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class MenuSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;

        public MenuSeedData(ISugarUnitOfWork<DbContext> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public void Initialize()
        {
            using (var context = _unitOfWork.CreateContext())
            {
                // 检查菜单表是否有数据
                var menuCount = context.Menus.AsQueryable().Count();
                if (menuCount > 0)
                {
                    return;
                }

                try
                {
                    // 创建并插入顶级菜单
                    var homeMenu = new Menu
                    {
                        Name = "首页",
                        Path = "/home",
                        Icon = "HomeFilled",
                        Sort = 1,
                        IsHidden = false,
                        ParentId = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    var systemMenu = new Menu
                    {
                        Name = "系统管理",
                        Path = "/system",
                        Icon = "Setting",
                        Sort = 2,
                        IsHidden = false,
                        ParentId = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    var monitorMenu = new Menu
                    {
                        Name = "监控管理",
                        Path = "/monitor",
                        Icon = "Monitor",
                        Sort = 3,
                        IsHidden = false,
                        ParentId = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    var toolMenu = new Menu
                    {
                        Name = "系统工具",
                        Path = "/tool",
                        Icon = "Tools",
                        Sort = 4,
                        IsHidden = false,
                        ParentId = 0,
                        CreateBy = "system",
                        UpdateBy = "system"
                    };

                    // 插入顶级菜单
                    context.Menus.Insert(homeMenu);
                    context.Menus.Insert(systemMenu);
                    context.Menus.Insert(monitorMenu);
                    context.Menus.Insert(toolMenu);
                    
                    // 提交事务以获取自动生成的ID
                    context.Commit();
                    
                    // 查询插入的菜单ID
                    var home = context.Menus.GetFirst(m => m.Name == "首页");
                    var system = context.Menus.GetFirst(m => m.Name == "系统管理");
                    var monitor = context.Menus.GetFirst(m => m.Name == "监控管理");
                    var tool = context.Menus.GetFirst(m => m.Name == "系统工具");

                    // 创建系统管理子菜单
                    var systemChildren = new List<Menu>
                    {
                        new Menu
                        {
                            Name = "用户管理",
                            Path = "/system/user",
                            Icon = "User",
                            ParentId = system.Id,
                            Sort = 1,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "角色管理",
                            Path = "/system/role",
                            Icon = "UserFilled",
                            ParentId = system.Id,
                            Sort = 2,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "菜单管理",
                            Path = "/system/menu",
                            Icon = "Menu",
                            ParentId = system.Id,
                            Sort = 3,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "部门管理",
                            Path = "/system/dept",
                            Icon = "Notebook",
                            ParentId = system.Id,
                            Sort = 4,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "岗位管理",
                            Path = "/system/post",
                            Icon = "Memo",
                            ParentId = system.Id,
                            Sort = 5,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }
                    };

                    // 创建监控管理子菜单
                    var monitorChildren = new List<Menu>
                    {
                        new Menu
                        {
                            Name = "在线用户",
                            Path = "/monitor/online",
                            Icon = "Connection",
                            ParentId = monitor.Id,
                            Sort = 1,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "操作日志",
                            Path = "/monitor/log",
                            Icon = "Document",
                            ParentId = monitor.Id,
                            Sort = 2,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "系统性能",
                            Path = "/monitor/server",
                            Icon = "Cpu",
                            ParentId = monitor.Id,
                            Sort = 3,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }
                    };

                    // 创建系统工具子菜单
                    var toolChildren = new List<Menu>
                    {
                        new Menu
                        {
                            Name = "代码生成",
                            Path = "/tool/gen",
                            Icon = "Edit",
                            ParentId = tool.Id,
                            Sort = 1,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "系统接口",
                            Path = "/tool/swagger",
                            Icon = "Link",
                            ParentId = tool.Id,
                            Sort = 2,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }
                    };

                    // 插入子菜单
                    context.Menus.InsertRange(systemChildren);
                    context.Menus.InsertRange(monitorChildren);
                    context.Menus.InsertRange(toolChildren);

                    // 提交所有更改
                    context.Commit();
                    
                    Console.WriteLine("菜单种子数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"菜单种子数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 