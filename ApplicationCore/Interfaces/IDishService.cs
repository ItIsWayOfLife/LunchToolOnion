using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
   public interface IDishService
    {
        void AddDish(DishDTO dishDTO);
        void DeleteDish(int? id);
        void EditDish(DishDTO dishDTO);
        DishDTO GetDish(int? id);
        IEnumerable<DishDTO> GetDishes(int? menuId);
        void Dispose();
    }
}
