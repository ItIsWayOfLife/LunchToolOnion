using ApplicationCore.DTO;
using ApplicationCore.Entities;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;

namespace ApplicationCore.Services
{
    public class MenuService : IMenuService
    {
        private IUnitOfWork Database { get; set; }

        public MenuService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public IEnumerable<MenuDTO> GetMenus(int? providerId)
        {
            if (providerId == null)
                throw new ValidationException("Не установлено id поставщика", "");
            var provider = Database.Provider.Get(providerId.Value);
            if (provider == null)
                throw new ValidationException("Поставщик не найден", "");

            // применяем автомаппер для проекции одной коллекции на другую
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<Menu, MenuDTO>()).CreateMapper();
            var menus = mapper.Map<IEnumerable<Menu>, List<MenuDTO>>(Database.Menu.GetAll());

            return menus.Where(p => p.ProviderId == providerId).ToList();
        }

        public void AddMenu(MenuDTO menuDTO)
        {
            if (menuDTO.Name == null)
                throw new ValidationException("Название не установлено", "");

            Menu menu = new Menu()
            {
                Date = menuDTO.Date,
                Info = menuDTO.Info,
                Name = menuDTO.Name,
                ProviderId = menuDTO.ProviderId
            };

            Database.Menu.Create(menu);
            Database.Save();
        }

        public void DeleteMenu(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id меню", "");
            var provider = Database.Menu.Get(id.Value);
            if (provider == null)
                throw new ValidationException("Меню не найдено", "");

            Database.Menu.Delete(id.Value);
            Database.Save();
        }

        public MenuDTO GetMenu(int? id)
        {
            if (id == null)
                throw new ValidationException("Не установлено id меню", "");
            var menu = Database.Menu.Get(id.Value);
            if (menu == null)
                throw new ValidationException("Меню не найдено", "");

            MenuDTO menuDTO = new MenuDTO()
            {
                Id = menu.Id,
                Info = menu.Info,
                Date = menu.Date,
                Name = menu.Name,
                ProviderId = menu.ProviderId
            };

            return menuDTO;
        }

        public void EditMenu(MenuDTO menuDTO)
        {
            if (menuDTO.Name == null)
                throw new ValidationException("Название не установлено", "");           

            Menu menu = Database.Menu.Get(menuDTO.Id);

            if (menu == null)
            {
                throw new ValidationException("Меню не найдено", "");
            }

            menu.Info = menuDTO.Info;
            menu.Name = menuDTO.Name;
            menu.Date = menuDTO.Date;

            Database.Menu.Update(menu);
            Database.Save();
        }

        public void Dispose()
        {
            Database.Dispose();
        }
    }
}
