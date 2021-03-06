﻿using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class CatalogService : ICatalogService
    {
        private IUnitOfWork Database { get; set; }

        public CatalogService(IUnitOfWork uow)
        {
            Database = uow;
        }


        public IEnumerable<CatalogDTO> GetСatalogs()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Catalog, CatalogDTO>()).CreateMapper();
            var сatalogs = mapper.Map<IEnumerable<Catalog>, List<CatalogDTO>>(Database.Catalog.GetAll());

            return сatalogs;
        }

        public IEnumerable<CatalogDTO> GetСatalogs(int? providerId)
        {
            if (providerId == null)
                throw new ValidationException("Не установлено id поставщика", "");
            var provider = Database.Provider.Get(providerId.Value);
            if (provider == null)
                throw new ValidationException("Поставщик не найден", "");

            return GetСatalogs().Where(p => p.ProviderId == providerId).ToList();
        }

        public void AddСatalog(CatalogDTO сatalogDTO)
        {
            if (сatalogDTO.Name == null)
                throw new ValidationException("Название не установлено", "");

            Catalog menu = new Catalog()
            {
                Info = сatalogDTO.Info,
                Name = сatalogDTO.Name,
                ProviderId = сatalogDTO.ProviderId
            };

            Database.Catalog.Create(menu);
            Database.Save();
        }

        public void DeleteСatalog(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id каталога", "");
            var provider = Database.Catalog.Get(id.Value);
            if (provider == null)
                throw new ValidationException("Каталог не найдено", "");

            var dishesInMenu = Database.MenuDishes.GetAll().Where(p => p.Dish.CatalogId == id.Value);

            foreach (var dishInMenu in dishesInMenu)
            {
                Database.MenuDishes.Delete(dishInMenu.Id);
            }

            Database.Catalog.Delete(id.Value);
            Database.Save();
        }

        public CatalogDTO GetСatalog(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id каталога", "");
            var сatalog = Database.Catalog.Get(id.Value);
            if (сatalog == null)
                throw new ValidationException("Каталог не найдено", "");

            CatalogDTO сatalogDTO = new CatalogDTO()
            {
                Id = сatalog.Id,
                Info = сatalog.Info,
                Name = сatalog.Name,
                ProviderId = сatalog.ProviderId
            };

            return сatalogDTO;
        }

        public void EditСatalog(CatalogDTO сatalogDTO)
        {
            if (сatalogDTO.Name == null)
                throw new ValidationException("Название не установлено", "");           

            Catalog сatalog = Database.Catalog.Get(сatalogDTO.Id);

            if (сatalog == null)
            {
                throw new ValidationException("Каталог не найдено", "");
            }

            сatalog.Info = сatalogDTO.Info;
            сatalog.Name = сatalogDTO.Name;

            Database.Catalog.Update(сatalog);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
