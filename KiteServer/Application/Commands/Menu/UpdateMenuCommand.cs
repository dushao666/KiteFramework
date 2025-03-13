using MediatR;

namespace Application.Commands.Menu
{
    public class UpdateMenuCommand : IRequest<bool>
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
    }
} 