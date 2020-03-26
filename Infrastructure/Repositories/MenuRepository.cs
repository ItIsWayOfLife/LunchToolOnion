using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Repositories 
{
    public class MenuRepository : IRepository<Menu>
    {
        private readonly ApplicationContext _applicationContext;

        public MenuRepository(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public void Create(Menu item)
        {
            _applicationContext.Menus.Add(item);
        }

        public void Delete(int id)
        {
            Menu menu = _applicationContext.Menus.Find(id);
            if (menu != null)
            {
                _applicationContext.Menus.Remove(menu);
            }
        }

        public IEnumerable<Menu> Find(Func<Menu, bool> predicate)
        {
            return _applicationContext.Menus.Include(p => p.Provider).Where(predicate).ToList();
        }

        public Menu Get(int id)
        {
            return _applicationContext.Menus.Find(id);
        }

        public IEnumerable<Menu> GetAll()
        {
            return _applicationContext.Menus.Include(p => p.Provider);
        }

        public void Update(Menu item)
        {
            _applicationContext.Entry(item).State = EntityState.Modified;
        }
    }
}
