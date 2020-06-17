using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using WebAPI.Models;

namespace WebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class MenuController : ControllerBase
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
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

                return new ObjectResult(menuModels);
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
                var menu = _menuService.GetMenu(id);
                return new ObjectResult(ConvertMenuDTOToMenuModel(menu));
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet, Route("provider/{providerId}")]
        public IActionResult GetByProviderId(int providerId)
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetMenus(providerId);
                List<MenuModel> menuModels = new List<MenuModel>();

                foreach (var m in menuDTOs)
                {
                    menuModels.Add(ConvertMenuDTOToMenuModel(m));
                }
                return new ObjectResult(menuModels);
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
                _menuService.DeleteMenu(id);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult Put(MenuModel model)
        {
            try
            {
                _menuService.EditMenu(ConvertMenuModelToMenuDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {

                return BadRequest(ex);
            }
        }

        [HttpPost]
        public IActionResult Post(MenuModel model)
        {
            try
            {
                _menuService.AddMenu(ConvertMenuModelToMenuDTO(model));
                return Ok(model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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

