namespace DomainShared.Dto.System
{
    /// <summary>
    /// 菜单数据传输对象
    /// </summary>
    public class MenuDto
    {
        /// <summary>
        /// 菜单ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 父菜单ID
        /// </summary>
        public long? ParentId { get; set; }

        /// <summary>
        /// 显示顺序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// 组件路径
        /// </summary>
        public string Component { get; set; }

        /// <summary>
        /// 路由元数据
        /// </summary>
        public MenuMetaDto Meta { get; set; }

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuDto> Children { get; set; }
    }

    /// <summary>
    /// 菜单元数据传输对象
    /// </summary>
    public class MenuMetaDto
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 是否需要认证
        /// </summary>
        public bool RequiresAuth { get; set; } = true;

        /// <summary>
        /// 是否缓存组件
        /// </summary>
        public bool KeepAlive { get; set; } = false;

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 允许访问的角色
        /// </summary>
        public List<string> Roles { get; set; }
    }
} 