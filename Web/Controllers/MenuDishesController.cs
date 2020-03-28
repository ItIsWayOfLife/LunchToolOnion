using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using Web.Models.MenuDishes;

namespace Web.Controllers
{
    public class MenuDishesController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly ILogger<MenuController> _logger;

        public MenuDishesController(IMenuService menuService, ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index(int? menuId, string searchSelectionString, string name, SortStateMenuDishes sortMenuDish = SortStateMenuDishes.PriceAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Dish/Index");

            try
            {
                var manu = _menuService.GetMenu(menuId);

                if (manu == null)
                    throw new ValidationException($"Меню {menuId} не найдено", "");

                ViewData["DateCatalog"] = "" + manu.Date.Date;

                IEnumerable<MenuDishesDTO> menuDishesDTOs = _menuService.GetMenuDishes(menuId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesViewModel>()).CreateMapper();
                var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesViewModel>>(menuDishesDTOs);

                // элементы поиска
                List<string> searchSelection = new List<string>() { "Поиск по", "Названию", "Информации", "Весу", "Цене" };

                // простой поиск
                switch (searchSelectionString)
                {
                    case "Названию":
                        menuDishes = menuDishes.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
                        break;
                    case "Информации":
                        menuDishes = menuDishes.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();
                        break;
                    case "Весу":
                        menuDishes = menuDishes.Where(t => t.Weight.ToString() == name).ToList();
                        break;
                    case "Цене":
                        menuDishes = menuDishes.Where(t => t.Price.ToString() == name).ToList();
                        break;
                }

                ViewData["PriceSort"] = sortMenuDish == SortStateMenuDishes.PriceAsc ? SortStateMenuDishes.PriceDesc : SortStateMenuDishes.PriceAsc;

                menuDishes = sortMenuDish switch
                {
                    SortStateMenuDishes.PriceDesc => menuDishes.OrderByDescending(s => s.Price).ToList(),
                    _ => menuDishes.OrderBy(s => s.Price).ToList(),
                };

                return View(new ListMenuDishViewModel()
                {
                    MenuId = menuId.Value,
                    MenuDishes = menuDishes,
                    SeacrhString = name,
                    SearchSelection = new SelectList(searchSelection),
                    SearchSelectionString = searchSelectionString,
                     ProviderId = manu.ProviderId
                });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }
    }
}
