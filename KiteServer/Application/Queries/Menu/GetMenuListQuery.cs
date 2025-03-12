using Application.Dtos;
using MediatR;

namespace Application.Queries.Menu
{
    public class GetMenuListQuery : IRequest<List<MenuDto>>
    {
        /// <summary>
        /// 是否包含隐藏菜单
        /// </summary>
        public bool IncludeHidden { get; set; }

        /// <summary>
        /// 关键字搜索
        /// </summary>
        public string Keyword { get; set; }
    }
} 