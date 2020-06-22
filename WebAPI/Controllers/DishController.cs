using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using AutoMapper;
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
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IMenuService _menuService;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<DishController> _logger;

        private readonly string _path;

        public DishController(IDishService dishService,
            IMenuService menuService,
            IUserHelper userHelper,
            ILogger<DishController> logger)
        {
            _dishService = dishService;        
            _menuService = menuService;
            _userHelper = userHelper;
            _logger = logger;

            _path = PathConstants.APIURL + PathConstants.pathForAPI;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                IEnumerable<DishDTO> dishDTOs = _dishService.GetAllDishes();
                List<DishModel> dishModels = new List<DishModel>();

                foreach (var d in dishDTOs)
                {
                    dishModels.Add(ConvertDishDTOToDishModel(d));
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/get]:[info:get dishes]");

                return new ObjectResult(dishModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var dish = _dishService.GetDish(id);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/get/{id}]:[info:get dish {id}]");

                return new ObjectResult(ConvertDishDTOToDishModel(dish));
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet, Route("catalog/{catalogid}")]
        public IActionResult GetByCatalogId(int catalogid)
        {
            try
            {
                IEnumerable<DishDTO> dishDTOs = _dishService.GetDishes(catalogid);
                List<DishModel> dishModels = new List<DishModel>();

                foreach (var d in dishDTOs)
                {
                    dishModels.Add(ConvertDishDTOToDishModel(d));
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/catalog/{catalogid}]:[info:get dishes by catalog {catalogid}]");

                return new ObjectResult(dishModels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/catalog/{catalogid}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            try
            {
                _dishService.DeleteDish(id);

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/delete/{id}]:[info:delete dish {id}]:[user:{userId}]");

                return Ok(id);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/delete/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPut]
        [Authorize(Roles = "admin")]
        public IActionResult Put(DishModel model)
        {
            try
            {
                _dishService.EditDish(ConvertDishModelToDishDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/put]:[info:edit dish {model.Id}]:[user:{userId}]");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/put]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Post(DishModel model)
        {
            try
            {
                _dishService.AddDish(ConvertDishModelToDishDTO(model));

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/put]:[info:create dish {model.Name}]:[user:{userId}]");

                return Ok(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/post]:[error:{ex}]");

                return BadRequest();
            }
        }


        [HttpGet, Route("menudishes/{menuId}")]
        public IActionResult GetDishesByMenuId(int menuId)
        {
            try
            {
                var menuDishesDTOs = _menuService.GetMenuDishes(menuId).ToList();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesModel>()).CreateMapper();
                var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesModel>>(menuDishesDTOs);

                for (int i = 0; i < menuDishesDTOs.Count(); i++)
                {
                    menuDishes[i].Path = _path + menuDishesDTOs[i].Path;
                    menuDishes[i].DishId = menuDishesDTOs[i].DishId.Value;
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[dish/menudishes/{menuId}]:[info:get menu dishes by menu {menuId}]");

                return new ObjectResult(menuDishes);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[dish/menudishes/{menuId}]:[error:{ex}]");

                return BadRequest();
            }
        }


        private DishDTO ConvertDishModelToDishDTO(DishModel model)
        {
            return new DishDTO()
            {
                Id = model.Id,
                Info = model.Info,
                CatalogId = model.CatalogId,
                Name = model.Name,
                Price = model.Price,
                Weight = model.Weight,
                Path = model.Path.Replace(_path, "")
            };
        }

        private DishModel ConvertDishDTOToDishModel(DishDTO dto)
        {
            return new DishModel()
            {
                Id = dto.Id,
                Info = dto.Info,
                AddMenu = dto.AddMenu,
                CatalogId = dto.CatalogId,
                Name = dto.Name,
                Path = _path +dto.Path,
                Price = dto.Price,
                Weight = dto.Weight
            };
        }
     
    }
}
