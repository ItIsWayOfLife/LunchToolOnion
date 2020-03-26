using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class DishService : IDishService
    {
        private IUnitOfWork Database { get; set; }

        public DishService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<DishDTO> GetDishes(int? menuId)
        {
            if (menuId == null)
                throw new ValidationException("Не установлен id меню", "");
            var menu = Database.Menu.Get(menuId.Value);
            if (menu == null)
                throw new ValidationException("Меню не найдено", "");

            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Dish, DishDTO>()).CreateMapper();
            var dishes = mapper.Map<IEnumerable<Dish>, List<DishDTO>>(Database.Dish.GetAll());

            return dishes.Where(p => p.MenuId == menuId).ToList();
        }

        public void AddDish(DishDTO dishDTO)
        {
            if (dishDTO.Name == null)
                throw new ValidationException("Название не установлено", "");

            if (dishDTO.Info == null)
                throw new ValidationException("Информация не установлена", "");

            Dish dish = new Dish()
            {
                MenuId = dishDTO.MenuId,
                Info = dishDTO.Info,
                Name = dishDTO.Name,
                Path = dishDTO.Path,
                Price = dishDTO.Price,
                Weight = dishDTO.Weight
            };

            Database.Dish.Create(dish);
            Database.Save();
        }

        public void DeleteDish(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id блюда", "");
            var provider = Database.Dish.Get(id.Value);
            if (provider == null)
                throw new ValidationException("Блюдо не найдено", "");

            Database.Dish.Delete(id.Value);
            Database.Save();
        }

        public DishDTO GetDish(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id блюда", "");
            var dish = Database.Dish.Get(id.Value);
            if (dish == null)
                throw new ValidationException("Блюдо не найдено", "");

            DishDTO dishDTO = new DishDTO()
            {
                Id = dish.Id,
                Info = dish.Info,
                Name = dish.Name,
                MenuId = dish.MenuId,
                Path = dish.Path,
                Price = dish.Price,
                Weight = dish.Weight
            };

            return dishDTO;
        }

        public void EditDish(DishDTO dishDTO)
        {
            if (dishDTO.Name == null)
                throw new ValidationException("Название не установлено", "");

            if (dishDTO.Info == null)
                throw new ValidationException("Информация не установлена", "");

            Dish dish = Database.Dish.Get(dishDTO.Id);

            if (dish == null)
            {
                throw new ValidationException("Блюдо не найдено", "");
            }

            dish.Info = dishDTO.Info;
            dish.Name = dishDTO.Name;
            dish.Path = dishDTO.Path;
            dish.Price = dishDTO.Price;
            dish.Weight = dishDTO.Weight;

            Database.Dish.Update(dish);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
