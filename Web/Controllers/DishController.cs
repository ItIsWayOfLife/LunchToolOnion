using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Dish;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class DishController:Controller
    {
        private readonly IDishService _dishService;
        private readonly IСatalogService _сatalogService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IMenuService _menuService;

        private readonly PathConstants _pathConstants;

        private readonly string _path;

        private readonly ILogger<DishController> _logger;

        public DishController(IDishService dishService, IWebHostEnvironment appEnvironment,
             IСatalogService сatalogService,
             IMenuService menuService,
        ILogger<DishController> logger) 
        {
            _dishService = dishService;
            _appEnvironment = appEnvironment;
            _сatalogService = сatalogService;
            _menuService = menuService;
            _logger = logger;
            _pathConstants = new PathConstants();
            _path = _pathConstants.pathDish;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int? catalogId, int? menuId, string searchSelectionString, string name, SortState sortDish = SortState.PriceAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Dish/Index");

            try
            {
                var catalog = _сatalogService.GetСatalog(catalogId);

                if (catalog == null)
                    throw new ValidationException($"Каталог {catalogId} не найдено", "");
                
                ViewData["NameCatalog"] = "" + catalog.Name;

                IEnumerable<DishDTO> providersDtos;
                List<int> addedDish = new List<int>();

                if (menuId != null)
                {
                    addedDish = _menuService.GetMenuIdDishes(menuId);
                    providersDtos = _dishService.GetDishesForMenu(catalogId, addedDish);
                }
                else
                {
                    providersDtos = _dishService.GetDishes(catalogId);
                }
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<DishDTO, DishViewModel>()).CreateMapper();
                var dishes = mapper.Map<IEnumerable<DishDTO>, List<DishViewModel>>(providersDtos);

                foreach (var d in dishes)
                {
                    d.Path = _path + d.Path;
                }

                // элементы поиска
                List<string> searchSelection = new List<string>() { "Поиск по", "Названию", "Информации", "Весу", "Цене" };

                if (name == null)
                    name = "";

                    // простой поиск
                    switch (searchSelectionString)
                    {
                        case "Названию":
                            dishes = dishes.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
                            break;
                        case "Информации":
                            dishes = dishes.Where(e => e.Info.ToLower().Contains(name.ToLower())).ToList();
                            break;
                        case "Весу":
                            dishes = dishes.Where(t => t.Weight.ToString() == name).ToList();
                            break;
                        case "Цене":
                            dishes = dishes.Where(t => t.Price.ToString() == name).ToList();
                            break;
                    }

                ViewData["PriceSort"] = sortDish == SortState.PriceAsc ? SortState.PriceDesc : SortState.PriceAsc;

                dishes = sortDish switch
                {
                    SortState.PriceDesc => dishes.OrderByDescending(s => s.Price).ToList(),
                    _ => dishes.OrderBy(s => s.Price).ToList(),
                };
             
                return View(new ListDishViewModel()
                {
                     MenuId = menuId,
                    Dishes = dishes,
                    CatalogId = catalogId.Value,
                     AddedDish = addedDish,
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

        [HttpPost]
        public IActionResult MakeMenu(int menuId, List<int> newAddedDishes, List<int> allSelect)
        {
            try
            {
                _menuService.MakeMenu(menuId, newAddedDishes, allSelect);

                _logger.LogInformation($"{DateTime.Now.ToString()}: Make menu for {menuId}");
                return RedirectToAction("Index", "MenuDishes", new { menuId = menuId });
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return Content(ex.Message);
            }

        }

        [HttpGet]
        public IActionResult Add(int catalogId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Dish/Add");

            return View(new AddDishViewModel() { CatalogId = catalogId });
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm]AddDishViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DishDTO dishDTO = null;
                    string path = null;

                    // сохранение картинки
                    if (uploadedFile != null)
                    {
                        path = uploadedFile.FileName;
                        // сохраняем файл в папку files/provider/ в каталоге wwwroot=
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath +_path + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                            _logger.LogInformation($"{DateTime.Now.ToString()}: Save image {path} in {_path}");
                        }
                    }

                    dishDTO = new DishDTO
                    {
                        Info = model.Info,
                        CatalogId = model.CatalogId,
                        Name = model.Name,
                        Path = path,
                        Price = model.Price,
                        Weight = model.Weight
                    };

                    _dishService.AddDish(dishDTO);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} added new dish");

                    return RedirectToAction("Index",new { dishDTO.CatalogId });
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
        public IActionResult Delete(int? id, int catalogId, string searchSelectionString, string name)
        {
            try
            {
                _dishService.DeleteDish(id);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted dish {id}");

                return RedirectToAction("Index", new { catalogId, searchSelectionString, name });
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                return Content(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Dish/Edit");

            try
            {
                DishDTO dishDTO = _dishService.GetDish(id);

                var provider = new EditDithViewModel()
                {
                    Info = dishDTO.Info,
                    Id = dishDTO.Id,
                    Name = dishDTO.Name,
                    Path = _path +dishDTO.Path,
                    Price = dishDTO.Price,
                    Weight = dishDTO.Weight,
                    CatalogId = dishDTO.CatalogId
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
        public async Task<ActionResult> Edit(IFormFile uploadedFile, [FromForm]EditDithViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    DishDTO dishDTO = null;
                    string path = null;

                    // сохранение картинки
                    if (uploadedFile != null)
                    {
                        path = uploadedFile.FileName;
                        // сохраняем файл в папку files/provider/ в каталоге wwwroot
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath +_path + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                            _logger.LogInformation($"{DateTime.Now.ToString()}: Save image {path}");
                        }
                    }
                    else
                    {
                        path = model.Path;
                    }

                    dishDTO = new DishDTO
                    {
                        Id = model.Id,
                        Info = model.Info,
                        Name = model.Name,
                        Path = path.Replace(_path, ""),
                        Price = model.Price,
                        Weight = model.Weight,
                        CatalogId = model.CatalogId
                    };

                    _dishService.EditDish(dishDTO);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edit dish {model.Id}");

                    return RedirectToAction("Index", new { dishDTO.CatalogId });
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
