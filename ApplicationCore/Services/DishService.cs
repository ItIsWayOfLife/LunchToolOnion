﻿using ApplicationCore.DTO;
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

        public IEnumerable<DishDTO> GetAllDishes()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Dish, DishDTO>()).CreateMapper();
            var dishes = mapper.Map<IEnumerable<Dish>, List<DishDTO>>(Database.Dish.GetAll());

            return dishes;
        }

        public IEnumerable<DishDTO> GetDishesForMenu(int? catalogId, List<int> addedDishes)
        {
            if (catalogId == null)
                throw new ValidationException("Не установлен id каталога", "");

            var сatalog = Database.Catalog.Get(catalogId.Value);

            if (сatalog == null)
                throw new ValidationException("Каталог не найдено", "");

            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Dish, DishDTO>()).CreateMapper();
            var dishes = mapper.Map<IEnumerable<Dish>, List<DishDTO>>(Database.Dish.GetAll());

            foreach (var dish in dishes)
            {
                if (addedDishes.Contains(dish.Id))
                {
                    dish.AddMenu = true;
                }
                else
                {
                    dish.AddMenu = false;
                }
            }

            return dishes.Where(p => p.CatalogId == catalogId).ToList();
        }

        public IEnumerable<DishDTO> GetDishes(int? catalogId)
        {
            if (catalogId == null)
                throw new ValidationException("Не установлен id каталога", "");
            var сatalog = Database.Catalog.Get(catalogId.Value);
            if (сatalog == null)
                throw new ValidationException("Каталог не найдено", "");

            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Dish, DishDTO>()).CreateMapper();
            var dishes = mapper.Map<IEnumerable<Dish>, List<DishDTO>>(Database.Dish.GetAll());

            return dishes.Where(p => p.CatalogId == catalogId).ToList();
        }

        public void AddDish(DishDTO dishDTO)
        {
            if (dishDTO.Name == null)
                throw new ValidationException("Название не установлено", "");

            if (dishDTO.Info == null)
                throw new ValidationException("Информация не установлена", "");

            Dish dish = new Dish()
            {
                CatalogId = dishDTO.CatalogId,
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


            var dishesInMenu = Database.MenuDishes.GetAll().Where(p=>p.DishId==id.Value);
           
            foreach (var dishInMenu in dishesInMenu)
            {
                Database.MenuDishes.Delete(dishInMenu.Id);
            }

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
                CatalogId = dish.CatalogId,
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
