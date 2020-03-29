using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class ProviderService : IProviderService
    {
        private IUnitOfWork Database { get; set; }

        public ProviderService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<ProviderDTO> GetProviders()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Provider, ProviderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Provider>, List<ProviderDTO>>(Database.Provider.GetAll());
        }

        public IEnumerable<ProviderDTO> GetFavoriteProviders()
        {
            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Provider, ProviderDTO>()).CreateMapper();
            return mapper.Map<IEnumerable<Provider>, List<ProviderDTO>>(Database.Provider.GetAll()).Where(f => f.IsFavorite == true);
        }

        public void AddProvider(ProviderDTO providerDTO)
        {
            if (providerDTO.Name == null)
                throw new ValidationException("Имя не установлено", "");

            if (providerDTO.Email == null)
                throw new ValidationException("Email не установлен", "");

            if (providerDTO.TimeWorkWith == null)
                throw new ValidationException("Время открытия не установлено", "");

            if (providerDTO.TimeWorkTo == null)
                throw new ValidationException("Время закрытия не установлено", "");

            Provider provider = new Provider()
            {
                Email = providerDTO.Email,
                Info = providerDTO.Info,
                IsActive = providerDTO.IsActive,
                IsFavorite = providerDTO.IsFavorite,
                Name = providerDTO.Name,
                Path = providerDTO.Path,
                TimeWorkTo = providerDTO.TimeWorkTo,
                TimeWorkWith = providerDTO.TimeWorkWith,
                WorkingDays = providerDTO.WorkingDays
            };

            Database.Provider.Create(provider);
            Database.Save();
        }

        public void DeleteProvider(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id поставщика", "");
            var provider = Database.Provider.Get(id.Value);
            if (provider == null)
                throw new ValidationException("Поставщик не найден", "");

            var dishesInMenu = Database.MenuDishes.GetAll().Where(p => p.Menu.ProviderId == id.Value);

            foreach (var dishInMenu in dishesInMenu)
            {
                Database.MenuDishes.Delete(dishInMenu.Id);
            }

            Database.Provider.Delete(id.Value);
            Database.Save();
        }

        public void EditProvider(ProviderDTO providerDTO)
        {
            if (providerDTO.Name == null)
                throw new ValidationException("Имя не установлено", "");

            if (providerDTO.Email == null)
                throw new ValidationException("Email не установлен", "");

            if (providerDTO.TimeWorkWith == null)
                throw new ValidationException("Время открытия не установлено", "");

            if (providerDTO.TimeWorkTo == null)
                throw new ValidationException("Время закрытия не установлено", "");

            Provider provider = Database.Provider.Get(providerDTO.Id);

            if (provider == null)
            {
                throw new ValidationException("Поставщик не найден", "");
            }

            provider.Email = providerDTO.Email;
            provider.Info = providerDTO.Info;
            provider.IsActive = providerDTO.IsActive;
            provider.IsFavorite = providerDTO.IsFavorite;
            provider.Name = providerDTO.Name;
            provider.Path = providerDTO.Path;
            provider.TimeWorkTo = providerDTO.TimeWorkTo;
            provider.TimeWorkWith = providerDTO.TimeWorkWith;
            provider.WorkingDays = providerDTO.WorkingDays;

            Database.Provider.Update(provider);
            Database.Save();
        }

        public ProviderDTO GetProvider(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id поставщика", "");
            var provider = Database.Provider.Get(id.Value);
            if (provider == null)
                throw new ValidationException("Поставщик не найден", "");

            ProviderDTO providerDTO = new ProviderDTO()
            {
                Id = provider.Id,
                WorkingDays = provider.WorkingDays,
                Email = provider.Email,
                Info = provider.Info,
                IsActive = provider.IsActive,
                IsFavorite = provider.IsFavorite,
                Name = provider.Name,
                Path = provider.Path,
                TimeWorkTo = provider.TimeWorkTo,
                TimeWorkWith = provider.TimeWorkWith
            };

            return providerDTO;
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
