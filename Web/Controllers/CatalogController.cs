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
using Web.Models.Catalog;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class CatalogController : Controller
    {
        private readonly ICatalogService _сatalogService;
        private readonly IProviderService _providerService;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ICatalogService сatalogService, IProviderService providerService,
            ILogger<CatalogController> logger)
        {
            _сatalogService = сatalogService;
            _logger = logger;
            _providerService = providerService;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int? providerId, int? menuId, string searchSelectionString, string name, SortState sortCatalog = SortState.NameAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Catalog/Index");

            try
            {
                IEnumerable<СatalogDTO> сatalogDTOs = _сatalogService.GetСatalogs(providerId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<СatalogDTO, CatalogViewModel>()).CreateMapper();
                var catalogs = mapper.Map<IEnumerable<СatalogDTO>, List<CatalogViewModel>>(сatalogDTOs);

                var provider = _providerService.GetProvider(providerId);

                if (provider==null)
                    throw new ValidationException("Поставщик не найден", "");

                ViewData["NameProvider"] = "" + provider.Name;

                // элементы поиска
                List<string> searchSelection = new List<string>() { "Поиск по", "Названию", "Информации"};

                if (name == null)
                    name = "";

                // простой поиск
                switch (searchSelectionString)
                {
                    case "Названию":
                        catalogs = catalogs.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
                        break;
                    case "Информации":
                        catalogs = catalogs.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();
                        break;
                }

                ViewData["NameSort"] = sortCatalog == SortState.NameAsc ? SortState.NameDesc : SortState.NameAsc;

                catalogs = sortCatalog switch
                {
                    SortState.NameDesc => catalogs.OrderByDescending(s => s.Name).ToList(),
                    _ => catalogs.OrderBy(s => s.Name).ToList(),
                };

                return View(new CatalogdProviderIdViewModel()
                {
                     MenuId = menuId,
                    Catalogs = catalogs,
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

        [HttpGet]
        public IActionResult Add(int providerId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Catalog/Add");

            return View(new AddCatalogViewModel() { ProviderId = providerId });
        }

        [HttpPost]
        public IActionResult Add(AddCatalogViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    СatalogDTO сatalogDTO = new СatalogDTO()
                    {
                        Info = model.Info,
                        Name = model.Name,
                        ProviderId = model.ProviderId
                    };

                    _сatalogService.AddСatalog(сatalogDTO);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} added new catalog");

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
                _сatalogService.DeleteСatalog(id);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted catalog {id}");

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
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Catalog/Edit");

            try
            {
                СatalogDTO сatalogDTO = _сatalogService.GetСatalog(id);
                if (сatalogDTO == null)
                throw new ValidationException("Каталог не найдено", "");

                var provider = new EditCatalogViewModel()
                {
                    Id = сatalogDTO.Id,
                    Info = сatalogDTO.Info,
                    Name = сatalogDTO.Name,
                    ProviderId = сatalogDTO.ProviderId
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
        public IActionResult Edit(EditCatalogViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    СatalogDTO сatalogDTO = new СatalogDTO
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Info = model.Info,
                        ProviderId = model.ProviderId
                    };

                    _сatalogService.EditСatalog(сatalogDTO);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited catalog {model.Id}");

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
