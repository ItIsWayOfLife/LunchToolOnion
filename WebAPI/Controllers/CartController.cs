using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using WebAPI.Controllers.Identity;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin, employee")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IUserHelper _userHelper;

        private readonly string _path;

        public CartController(ICartService cartService,
               UserManager<ApplicationUser> userManager,
                IUserHelper userHelper)
        {
            _cartService = cartService;
            _userHelper = userHelper;

            _path = PathConstants.APIURL + PathConstants.pathForAPI;
        }

        [HttpGet, Route("dishes")]
        public IActionResult GetDishes()
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                CartDTO cartDTO = _cartService.GetCart(userId);

                IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(userId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CartDishesDTO, CartDishesModel>()).CreateMapper();
                var cartDishes = mapper.Map<IEnumerable<CartDishesDTO>, List<CartDishesModel>>(cartDishDTO);

                foreach (var cD in cartDishes)
                {
                    cD.Path = _path + cD.Path;
                }

                return new ObjectResult(cartDishes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet, Route("fullprice")]
        public IActionResult GetFullPrice()
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                string fullPrice = _cartService.FullPriceCart(userId).ToString();

                return new ObjectResult(fullPrice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public IActionResult UpdateCartDishes(List<CartDishesUpdateModel> models)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                foreach (var m in models)
                {
                    _cartService.UpdateCountDishInCart(userId, m.Id, m.Count);
                }

                return Ok();
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
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _cartService.DeleteCartDish(id, userId);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost("{id}")]
        public IActionResult Post(int id)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _cartService.AddDishToCart(id, userId);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete, Route("all/delete")]
        public IActionResult DeleteAllDishesInCart(int id)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                _cartService.AllDeleteDishesToCart(userId);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
