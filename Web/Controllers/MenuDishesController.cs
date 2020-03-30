using ApplicationCore.Constants;
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
        private readonly IProviderService _providerService;
        private readonly ILogger<MenuController> _logger;
        private readonly ICatalogService _catalogService;

        private readonly PathConstants _pathConstants;

        private readonly string _path;

        public MenuDishesController(IMenuService menuService, IProviderService providerService,
            ICatalogService catalogService,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _providerService = providerService;
            _logger = logger;
            _pathConstants = new PathConstants();
            _path = _pathConstants.pathDish;
            _catalogService = catalogService;
        }

        [HttpGet]
        public IActionResult Index(int? menuId, string searchSelectionString, string name,
            string filterCatalog,
            SortStateMenuDishes sortMenuDish = SortStateMenuDishes.PriceAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request MenuDishes/Index");

            try
            {
                var menu = _menuService.GetMenu(menuId);

                if (menu == null)
                    throw new ValidationException($"Меню {menuId} не найдено", "");

                var provider = _providerService.GetProvider(menu.ProviderId);

                if (provider == null)
                    throw new ValidationException($"Поставщик {menu.ProviderId} не найдено", "");

                ViewData["NameMeniDishes"] = $"Блюда {provider.Name} на " + menu.Date.ToShortDateString();

                IEnumerable<MenuDishesDTO> menuDishesDTOs = _menuService.GetMenuDishes(menuId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesViewModel>()).CreateMapper();
                var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesViewModel>>(menuDishesDTOs);

                List<int> catalogFilterId = new List<int>() { -1};
                List<string> catalogFilterName = new List<string>() { "Все"};
               
                // элементы поиска
                List<string> searchSelection = new List<string>() { "Поиск по", "Названию", "Информации", "Весу", "Цене" };

                foreach (var mD in menuDishes)
                {
                    mD.Path = _path + mD.Path;

                    if (!catalogFilterId.Contains(mD.CatalogId))
                    {
                        catalogFilterId.Add(mD.CatalogId);
                        catalogFilterName.Add(_catalogService.GetСatalog(mD.CatalogId).Name);
                    }
                }

                // filter catalog
                if (filterCatalog != null && filterCatalog != catalogFilterName[0])
                {
                    int idCatalogFilterName = catalogFilterName.IndexOf(filterCatalog);
                    menuDishes = menuDishes.Where(p => p.CatalogId == catalogFilterId[idCatalogFilterName]).ToList();
                }
             
                if (name == null)
                    name = "";
                
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
                     FilterCategorySelection = new SelectList(catalogFilterName),
                    SearchSelectionString = searchSelectionString,
                     ProviderId = menu.ProviderId,
                      FilterCatalog = filterCatalog
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
