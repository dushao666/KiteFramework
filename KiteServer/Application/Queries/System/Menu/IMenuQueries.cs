using DomainShared.Dto.System;

namespace Application.Queries.System.Menu
{
    /// <summary>
    /// 菜单查询接口
    /// </summary>
    public interface IMenuQueries
    {
        /// <summary>
        /// 获取菜单树
        /// </summary>
        /// <param name="includeHidden">是否包含隐藏菜单</param>
        /// <returns>菜单树</returns>
        Task<List<MenuDto>> GetMenuTreeAsync(bool includeHidden = false);
        
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="keyword">关键字搜索</param>
        /// <param name="includeHidden">是否包含隐藏菜单</param>
        /// <returns>菜单列表</returns>
        Task<List<MenuDto>> GetMenuListAsync(string keyword = null, bool includeHidden = false);
    }
} 