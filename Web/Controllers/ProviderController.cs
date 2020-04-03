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
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Web.Models.Provider;

namespace Web.Controllers
{
    [Authorize(Roles = "admin")]
    public class ProviderController : Controller
    {
        private readonly IProviderService _providerService;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;
        private readonly PathConstants _pathConstants;

        private readonly ILogger<ProviderController> _logger;

        private readonly string _path;

        public ProviderController(IProviderService providerService, IWebHostEnvironment appEnvironment,
             IStringLocalizer<SharedResource> sharedLocalizer,
            ILogger<ProviderController> logger)
        {
            _providerService = providerService;
            _appEnvironment = appEnvironment;
            _logger = logger;
            _pathConstants = new PathConstants();
            _path = _pathConstants.pathProvider;
            _sharedLocalizer = sharedLocalizer;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string searchSelectionString, string name)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Provider/Index");

            try
            {
                IEnumerable<ProviderDTO> providersDtos = _providerService.GetProviders();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderViewModel>()).CreateMapper();
                var provider = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderViewModel>>(providersDtos);

                foreach (var pr in provider)
                {
                    pr.Path = _path + pr.Path;
                }

                List<string> searchSelection = new List<string>() { _sharedLocalizer["SearchBy"] };

                if (User.Identity.IsAuthenticated && User.IsInRole("admin"))
                {
                    searchSelection.Add(_sharedLocalizer["Id"]);
                }

                searchSelection.AddRange(new string[] { _sharedLocalizer["Name"], _sharedLocalizer["Email"], _sharedLocalizer["TimeWorkWith"], _sharedLocalizer["TimeWorkTo"], _sharedLocalizer["IsActive"], _sharedLocalizer["Inactive"] } );

                if (name == null)
                    name = "";


                if (searchSelectionString==_sharedLocalizer["Id"])
                    provider = provider.Where(n => n.Id.ToString().ToLower().Contains(name.ToLower())).ToList();
                else if (searchSelectionString == _sharedLocalizer["Name"])
                    provider = provider.Where(n => n.Name.ToLower().Contains(name.ToLower())).ToList();
                else if (searchSelectionString == _sharedLocalizer["Email"])
                    provider = provider.Where(e => e.Email.ToLower().Contains(name.ToLower())).ToList();
                else if (searchSelectionString == _sharedLocalizer["TimeWorkWith"])
                    provider = provider.Where(t => t.TimeWorkWith.ToShortTimeString().ToLower().Contains(name)).ToList();
                else if (searchSelectionString == _sharedLocalizer["TimeWorkTo"])
                    provider = provider.Where(t => t.TimeWorkTo.ToShortTimeString().ToLower().Contains(name)).ToList();
                else if (searchSelectionString == _sharedLocalizer["IsActive"])
                    provider = provider.Where(a => a.IsActive == true).ToList();
                else if (searchSelectionString == _sharedLocalizer["Inactive"])
                    provider = provider.Where(a => a.IsActive == false).ToList();

                return View(new ProviderListViewModel()
                {
                    ListProviders = new ListProviderViewModel() { Providers = provider },
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

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ListFavoriteProvider()
        { 
             _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Provider/ListFavoriteProvider");

            IEnumerable<ProviderDTO> providersDtos = _providerService.GetFavoriteProviders();
            var mapper = new MapperConfiguration(cfg => cfg.CreateMap<ProviderDTO, ProviderViewModel>()).CreateMapper();
            var provider = mapper.Map<IEnumerable<ProviderDTO>, List<ProviderViewModel>>(providersDtos);

            foreach (var pr in provider)
            {
                pr.Path = _path + pr.Path;
            }

            return View(new ListProviderViewModel() { Providers = provider });
        }

        #region For admin

        [HttpGet]
        public ActionResult Add()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Provider/Add");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile uploadedFile, [FromForm]AddProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ProviderDTO providerDto = null;
                    string path = null;

                    // сохранение картинки
                    if (uploadedFile != null)
                    {
                        path =  uploadedFile.FileName;
                        // сохраняем файл в папку files/provider/ в каталоге wwwroot
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath +_path +path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                            _logger.LogInformation($@"{DateTime.Now.ToString()}: Save image {path} in {_path}");
                        }
                    }

                    providerDto = new ProviderDTO
                    {
                        Email = model.Email,
                        Info = model.Info,
                        IsActive = model.IsActive,
                        IsFavorite = model.IsFavorite,
                        Name = model.Name,
                        Path = path, 
                        TimeWorkTo = model.TimeWorkTo,
                        TimeWorkWith = model.TimeWorkWith,
                        WorkingDays = model.WorkingDays
                    };

                    _providerService.AddProvider(providerDto);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} added new provider");

                    return RedirectToAction("Index");
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
        public ActionResult Delete(int? id)
        {
            try
            {
                _providerService.DeleteProvider(id);

                string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted provider {id}");

                return RedirectToAction("Index");
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
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Provider/Edit");

            try
            {
                ProviderDTO providerDto = _providerService.GetProvider(id);

                if (providerDto==null)
                    throw new ValidationException(_sharedLocalizer["ProviderNoFind"], "");


                var provider = new EditProviderViewModel()
                {
                    Id = providerDto.Id,
                    Email = providerDto.Email,
                    Info = providerDto.Info,
                    IsActive = providerDto.IsActive,
                    IsFavorite = providerDto.IsFavorite,
                    Name = providerDto.Name,
                    Path = _path+ providerDto.Path,
                    TimeWorkTo = providerDto.TimeWorkTo,
                    TimeWorkWith = providerDto.TimeWorkWith,
                    WorkingDays = providerDto.WorkingDays
                };

                return View(provider);
            }
            catch (ValidationException ex)
            {
                return Content(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> Edit(IFormFile uploadedFile, [FromForm]EditProviderViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ProviderDTO providerDto = null;
                    string path = null;

                    // сохранение картинки
                    if (uploadedFile != null)
                    {
                        path = uploadedFile.FileName;
                        // сохраняем файл в папку files/provider/ в каталоге wwwroot
                        using (var fileStream = new FileStream(_appEnvironment.WebRootPath + _path + path, FileMode.Create))
                        {
                            await uploadedFile.CopyToAsync(fileStream);
                            _logger.LogInformation($@"{DateTime.Now.ToString()}: Save image {path} in {_path}");
                        }
                    }
                    else
                    {
                        path = model.Path;
                    }

                    providerDto = new ProviderDTO
                    {
                        Id = model.Id,
                        Email = model.Email,
                        Info = model.Info,
                        IsActive = model.IsActive,
                        IsFavorite = model.IsFavorite,
                        Name = model.Name,
                        Path = path.Replace(_path,""), 
                        TimeWorkTo = model.TimeWorkTo,
                        TimeWorkWith = model.TimeWorkWith,
                        WorkingDays = model.WorkingDays
                    };

                    _providerService.EditProvider(providerDto);

                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} edited provider {model.Id}");

                    return RedirectToAction("Index");
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
