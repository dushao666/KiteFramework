using Domain.System;
using Microsoft.Extensions.Configuration;
using Repository.Repositories;

namespace Repository.Services.Seeds
{
    public class MenuSeedData : ISeedData
    {
        private readonly ISugarUnitOfWork<DbContext> _unitOfWork;
        private readonly IConfiguration _configuration;

        public MenuSeedData(ISugarUnitOfWork<DbContext> unitOfWork, IConfiguration configuration)
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
                    // 检查菜单表是否有数据
                    var menuCount = context.Menus.AsQueryable().Count();
                    if (menuCount > 0)
                    {
                        Console.WriteLine("菜单数据已存在，跳过初始化");
                        return;
                    }
                }

                try
                {
                    Console.WriteLine("开始初始化菜单数据...");
                    
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

                    // 插入顶级菜单并获取ID
                    var homeId = context.Menus.Context.Insertable(homeMenu).ExecuteReturnBigIdentity();
                    homeMenu.Id = homeId;
                    
                    var systemId = context.Menus.Context.Insertable(systemMenu).ExecuteReturnBigIdentity();
                    systemMenu.Id = systemId;
                    
                    var monitorId = context.Menus.Context.Insertable(monitorMenu).ExecuteReturnBigIdentity();
                    monitorMenu.Id = monitorId;
                    
                    var toolId = context.Menus.Context.Insertable(toolMenu).ExecuteReturnBigIdentity();
                    toolMenu.Id = toolId;
                    
                    context.Commit();
                    
                    Console.WriteLine($"创建顶级菜单: 首页(ID: {homeId}), 系统管理(ID: {systemId}), 监控管理(ID: {monitorId}), 系统工具(ID: {toolId})");

                    // 创建系统管理子菜单
                    var systemChildren = new List<Menu>
                    {
                        new Menu
                        {
                            Name = "用户管理",
                            Path = "/system/user",
                            Icon = "User",
                            Sort = 1,
                            IsHidden = false,
                            ParentId = systemId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "角色管理",
                            Path = "/system/role",
                            Icon = "UserFilled",
                            Sort = 2,
                            IsHidden = false,
                            ParentId = systemId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "菜单管理",
                            Path = "/system/menu",
                            Icon = "Menu",
                            Sort = 3,
                            IsHidden = false,
                            ParentId = systemId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "部门管理",
                            Path = "/system/dept",
                            Icon = "Notebook",
                            Sort = 4,
                            IsHidden = false,
                            ParentId = systemId,
                            CreateBy = "system",
                            UpdateBy = "system"
                        },
                        new Menu
                        {
                            Name = "岗位管理",
                            Path = "/system/post",
                            Icon = "Memo",
                            Sort = 5,
                            IsHidden = false,
                            ParentId = systemId,
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
                            ParentId = monitorId,
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
                            ParentId = monitorId,
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
                            ParentId = monitorId,
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
                            ParentId = toolId,
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
                            ParentId = toolId,
                            Sort = 2,
                            IsHidden = false,
                            CreateBy = "system",
                            UpdateBy = "system"
                        }
                    };

                    // 批量插入子菜单
                    context.Menus.InsertRange(systemChildren);
                    context.Menus.InsertRange(monitorChildren);
                    context.Menus.InsertRange(toolChildren);
                    context.Commit();
                    
                    Console.WriteLine($"创建了 {systemChildren.Count} 个系统管理子菜单");
                    Console.WriteLine($"创建了 {monitorChildren.Count} 个监控管理子菜单");
                    Console.WriteLine($"创建了 {toolChildren.Count} 个系统工具子菜单");
                    Console.WriteLine("菜单数据初始化成功");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"菜单数据初始化失败: {ex.Message}");
                    throw;
                }
            }
        }
    }
} 