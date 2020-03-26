using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Models.Menu;

namespace Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProviderService _providerService;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService, IProviderService providerService,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _logger = logger;
            _providerService = providerService;
        }

        [HttpGet]
        public IActionResult Index(int? providerId, string searchSelectionString, string name, SortState sortMenu = SortState.NameAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Index");

            try
            {
                IEnumerable<MenuDTO> menusDtos = _menuService.GetMenus(providerId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDTO, MenuViewModel>()).CreateMapper();
                var menus = mapper.Map<IEnumerable<MenuDTO>, List<MenuViewModel>>(menusDtos);

                var provider = _providerService.GetProvider(providerId);

                if (provider==null)
                    throw new ValidationException("Поставщик не найден", "");

                ViewData["NameProvider"] = "" + provider.Name;

                // элементы поиска
                List<string> searchSelection = new List<string>() { "Поиск по", "Названию", "Информации", "Дата добавления" };

                // простой поиск
                switch (searchSelectionString)
                {
                    case "Названию":
                        menus = menus.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
                        break;
                    case "Информации":
                        menus = menus.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();
                        break;
                    case "Дата добавления":
                        menus = menus.Where(t => t.Date.ToShortDateString().ToLower().Contains(name)).ToList();
                        break;
                }

                ViewData["NameSort"] = sortMenu == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;
                ViewData["DateSort"] = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

                menus = sortMenu switch
                {
                    SortState.NameDesc => menus.OrderByDescending(s => s.Name).ToList(),
                    SortState.DateAsc => menus.OrderBy(s => s.Date).ToList(),
                    SortState.DateDesc => menus.OrderByDescending(s => s.Date).ToList(),
                    _ => menus.OrderBy(s => s.Name).ToList(),
                };

                return View(new MenuAndProviderIdViewModel()
                {
                    Menus = menus,
                    ProviderId = providerId.Value,
                    SeacrhString = name,
                    SearchSelection = new SelectList(searchSelection),
                    SearchSelectionString = searchSelectionString
                });
            }
            catch (ValidationException ex)
            {
                ModelState.AddModelError(ex.Property, ex.Message);
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
            }

            return BadRequest("Некорректный запрос");
        }

        #region For admin

        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Add(int providerId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Add");

            return View(new AddMenuViewModel() { ProviderId = providerId });
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Add(AddMenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MenuDTO menuDTO = new MenuDTO()
                    {
                        Info = model.Info,
                        Name = model.Name,
                        ProviderId = model.ProviderId,
                        Date = model.Date
                    };

                    _menuService.AddMenu(menuDTO);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} added new menu");

                    return RedirectToAction("Index", new { model.ProviderId });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                }
            }
            return View(model);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public ActionResult Delete(int? id, int providerId, string searchSelectionString, string name)
        {
            try
            {
                _menuService.DeleteMenu(id);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted menu {id}");

                return RedirectToAction("Index", new { providerId, searchSelectionString, name });
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return Content(ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpGet]
        public ActionResult Edit(int? id)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Edit");

            try
            {
                MenuDTO menuDTO = _menuService.GetMenu(id);
                if (menuDTO==null)
                throw new ValidationException("Меню не найдено", "");

                var provider = new EditMenuViewModel()
                {
                    Id = menuDTO.Id,
                    Date = menuDTO.Date,
                    Info = menuDTO.Info,
                    Name = menuDTO.Name,
                    ProviderId = menuDTO.ProviderId
                };

                return View(provider);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return Content(ex.Message);
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Edit(EditMenuViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    MenuDTO menuDto = new MenuDTO
                    {
                        Id = model.Id,
                        Date = model.Date,
                        Name = model.Name,
                        Info = model.Info,
                        ProviderId = model.ProviderId
                    };

                    _menuService.EditMenu(menuDto);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited menu {model.Id}");

                    return RedirectToAction("Index", new { providerId = model.ProviderId });
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                }
            }
            return View(model);
        }

        #endregion
    }
}
