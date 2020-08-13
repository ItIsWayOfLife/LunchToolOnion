using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using WebAPI.Controllers.Identity;
using WebAPI.Models;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<MenuController> _logger;

        public MenuController(IMenuService menuService,
             IUserHelper userHelper,
            ILogger<MenuController> logger)
        {
            _menuService = menuService;
            _userHelper = userHelper;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetAllMenus();
                List<MenuModel> menuModels = new List<MenuModel>();

                foreach (var m in menuDTOs)
                {
                    menuModels.Add(ConvertMenuDTOToMenuModel(m));
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/get]:[info:get all menu]");

                return new ObjectResult(menuModels);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/get]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var menu = _menuService.GetMenu(id);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/get/{id}]:[info:get menu {id}]");

                return new ObjectResult(ConvertMenuDTOToMenuModel(menu));
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/get/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetMenus(providerid).OrderByDescending(p => p.Date);
                List<MenuModel> menuModels = new List<MenuModel>();

                foreach (var m in menuDTOs)
                {
                    menuModels.Add(ConvertMenuDTOToMenuModel(m));
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/provider/{providerid}]:[info:get menu by provider {providerid}]");

                return new ObjectResult(menuModels);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/provider/{providerid}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/provider/{providerid}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("dishes/{menuid}")]
        public IActionResult GetDishesInMenu(int menuid)
        {
            try
            {
                List<int> arrayIdDishes = new List<int>();

                arrayIdDishes = _menuService.GetMenuIdDishes(menuid);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/dishes/{menuid}]:[info:get dishes by menu {menuid}]");

                return new ObjectResult(arrayIdDishes);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/dishes/{menuid}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/dishes/{menuid}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(MenuModel model)
        {
            try
            {
                _menuService.AddMenu(ConvertMenuModelToMenuDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/post]:[info:create menu for {model.Date}]:[user:{userId}]");

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/post]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/post]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost, Route("makemenu")]
        [Authorize(Roles = "admin")]
        public IActionResult MakeMenu(MakeMenuModel model)
        {
            try
            {
                _menuService.MakeMenu(model.MenuId, model.NewAddedDishes, model.AllSelect);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/makemenu]:[info:make menu]:[user:{userId}]");

                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/makemenu]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/makemenu]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(MenuModel model)
        {
            try
            {
                _menuService.EditMenu(ConvertMenuModelToMenuDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/put]:[info:edit menu for {model.Date}]:[user:{userId}]");

                return Ok(model);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/put]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/put]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _menuService.DeleteMenu(id);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[menu/delete/{id}]:[info:delete menu {id}]:[user:{userId}]");

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/delete/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[menu/delete/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        private MenuDTO ConvertMenuModelToMenuDTO(MenuModel model)
        {
            return new MenuDTO()
            {
                Id = model.Id,
                Info = model.Info,
                Date = Convert.ToDateTime(model.Date),
                ProviderId = model.ProviderId
            };
        }

        private MenuModel ConvertMenuDTOToMenuModel(MenuDTO dto)
        {
            return new MenuModel()
            {
                Id = dto.Id,
                Info = dto.Info,
                ProviderId = dto.ProviderId,
                Date = dto.Date.ToShortDateString()
            };
        }

    }
}

