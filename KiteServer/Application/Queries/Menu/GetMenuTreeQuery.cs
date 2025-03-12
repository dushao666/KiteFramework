using Application.Dtos;
using MediatR;

namespace Application.Queries.Menu
{
    public class GetMenuTreeQuery : IRequest<List<MenuDto>>
    {
        /// <summary>
        /// 是否包含隐藏菜单
        /// </summary>
        public bool IncludeHidden { get; set; }
    }
} 