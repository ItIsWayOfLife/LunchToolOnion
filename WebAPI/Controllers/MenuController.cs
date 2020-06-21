﻿using ApplicationCore.DTO;
using ApplicationCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
                return BadRequest();
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
                return BadRequest();
            }
        }

        [HttpGet, Route("provider/{providerid}")]
        public IActionResult GetByProviderId(int providerid)
        {
            try
            {
                IEnumerable<MenuDTO> menuDTOs = _menuService.GetMenus(providerid).OrderByDescending(p=>p.Date);
                List<MenuModel> menuModels = new List<MenuModel>();

                foreach (var m in menuDTOs)
                {
                    menuModels.Add(ConvertMenuDTOToMenuModel(m));
                }
                return new ObjectResult(menuModels);
            }
            catch (Exception ex)
            {
                return BadRequest();
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
                return BadRequest();
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
                return BadRequest();
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
                return new ObjectResult(arrayIdDishes);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost, Route("makemenu")]
        public IActionResult MakeMenu(MakeMenuModel model)
        {
            try
            {
                _menuService.MakeMenu(model.MenuId, model.NewAddedDishes, model.AllSelect);
                return Ok();
            }
            catch (Exception ex)
            {
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

