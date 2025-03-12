using Application.Dtos;

namespace Application.IServices
{
    /// <summary>
    /// 菜单服务接口
    /// </summary>
    public interface IMenuService
    {
        Task<List<MenuDto>> GetMenuTreeAsync();
        Task<List<MenuDto>> GetMenuListAsync();
        Task<bool> AddMenuAsync(MenuDto menuDto);
        Task<bool> UpdateMenuAsync(MenuDto menuDto);
        Task<bool> DeleteMenuAsync(long id);
    }
} 