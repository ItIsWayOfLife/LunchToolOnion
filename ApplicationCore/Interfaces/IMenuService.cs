using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IMenuService
    {
        void AddMenu(MenuDTO menuDTO);
        void DeleteMenu(int? id);
        void EditMenu(MenuDTO menuDTO);
        MenuDTO GetMenu(int? id);
        IEnumerable<MenuDTO> GetMenus(int? providerId);
        IEnumerable<MenuDishesDTO> GetMenuDishes(int? menuId);
        void Dispose();
    }
}
