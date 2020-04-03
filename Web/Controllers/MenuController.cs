using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Web.Models.Menu;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class MenuController : Controller
    {
        private readonly IMenuService _menuService;
        private readonly IProviderService _providerService;
        private readonly ILogger<MenuController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        public MenuController(IMenuService menuService, IProviderService providerService,
            ILogger<MenuController> logger,
            IStringLocalizer<SharedResource> sharedLocalizer)
        {
            _menuService = menuService;
            _logger = logger;
            _providerService = providerService;
            _sharedLocalizer = sharedLocalizer;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int? providerId, string searchSelectionString, string name, SortState sortMenu = SortState.DateAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Index");

            try
            {
                IEnumerable<MenuDTO> menusDtos = _menuService.GetMenus(providerId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDTO, MenuViewModel>()).CreateMapper();
                var menus = mapper.Map<IEnumerable<MenuDTO>, List<MenuViewModel>>(menusDtos);

                var provider = _providerService.GetProvider(providerId);

                if (provider == null)
                    throw new ValidationException(_sharedLocalizer["ProviderNoFind"], "");

                ViewData["NameProvider"] = "" + provider.Name;

                // элементы поиска
                List<string> searchSelection = new List<string>() { _sharedLocalizer["SearchBy"], _sharedLocalizer["SearchInfo"], _sharedLocalizer["SearchDateAdd"] };

                if (name == null)
                    name = "";

                // простой поиск

                if (searchSelectionString== searchSelection[1])
                    menus = menus.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();
                else if (searchSelectionString == searchSelection[2])
                    menus = menus.Where(t => t.Date.ToShortDateString().Contains(name.ToLower())).ToList();

                ViewData["DateSort"] = sortMenu == SortState.DateAsc ? SortState.DateDesc : SortState.DateAsc;

                menus = sortMenu switch
                {
                    SortState.DateDesc => menus.OrderByDescending(s => s.Date).ToList(),
                    _ => menus.OrderBy(s => s.Date).ToList(),
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

            return BadRequest(_sharedLocalizer["BadRequest"]);
        }

        #region For admin
      
        [HttpGet]
        public IActionResult Add(int providerId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Add");

            return View(new AddMenuViewModel() { ProviderId = providerId });
        }

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

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Menu/Edit");

            try
            {
                MenuDTO menuDTO = _menuService.GetMenu(id);
                if (menuDTO == null)
                    throw new ValidationException(_sharedLocalizer["MenuNoFind"], "");

                var provider = new EditMenuViewModel()
                {
                    Id = menuDTO.Id,
                    Date = menuDTO.Date,
                    Info = menuDTO.Info,
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
