using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishController : ControllerBase
    {
        private readonly IDishService _dishService;
        private readonly IMenuService _menuService;
        private readonly PathConstants _pathConstants;

        private readonly string _path;

        public DishController(IDishService dishService,
            IMenuService menuService)
        {
            _dishService = dishService;
            _pathConstants = new PathConstants();
            _path = _pathConstants.APIURL+ _pathConstants.pathForAPI;
            _menuService = menuService;
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

                return new ObjectResult(dishModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var dish = _dishService.GetDish(id);
                return new ObjectResult(ConvertDishDTOToDishModel(dish));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet, Route("catalog/{catalogId}")]
        public IActionResult GetByCatalogId(int catalogId)
        {
            try
            {
                IEnumerable<DishDTO> dishDTOs = _dishService.GetDishes(catalogId);
                List<DishModel> dishModels = new List<DishModel>();

                foreach (var d in dishDTOs)
                {
                    dishModels.Add(ConvertDishDTOToDishModel(d));
                }

                return new ObjectResult(dishModels);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _dishService.DeleteDish(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult Put(DishModel model)
        {
            try
            {
                _dishService.EditDish(ConvertDishModelToDishDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Post(DishModel model)
        {
            try
            {
                _dishService.AddDish(ConvertDishModelToDishDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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


        [HttpGet, Route("menuDishes/{menuId}")]
        public IActionResult GetDishesByMenuId(int menuId)
        {
            try
            {
                IEnumerable<MenuDishesDTO> menuDishesDTOs = _menuService.GetMenuDishes(menuId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<MenuDishesDTO, MenuDishesModel>()).CreateMapper();
                var menuDishes = mapper.Map<IEnumerable<MenuDishesDTO>, List<MenuDishesModel>>(menuDishesDTOs);

                foreach (var m in menuDishes)
                {
                    m.Path = _path+m.Path;
                }

                return new ObjectResult(menuDishes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
