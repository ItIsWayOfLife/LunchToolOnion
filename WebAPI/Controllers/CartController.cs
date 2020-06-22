using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<CartController> _logger;

        private readonly string _path;

        public CartController(ICartService cartService,
                IUserHelper userHelper,
             ILogger<CartController> logger)
        {
            _cartService = cartService;
            _userHelper = userHelper;
            _logger = logger;

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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/dishes]:[info:get dishes in cart]:[user:{userId}]");

                return new ObjectResult(cartDishes);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/dishes]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/dishes]:[error:{ex}]");

                return BadRequest();
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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/fullprice]:[info:get full price]:[user:{userId}]");

                return new ObjectResult(fullPrice);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/fullprice]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/fullprice]:[error:{ex}]");

                return BadRequest();
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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/post/{id}]:[info:add dish {id} to cart]:[user:{userId}]");

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/post/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/post/{id}]:[error:{ex}]");

                return BadRequest();
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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/put]:[info:update cart]:[user:{userId}]");

                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/put]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/put]:[error:{ex}]");

                return BadRequest();
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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/delete/{id}]:[info:delete dish {id} with cart]:[user:{userId}]");

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/delete/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/delete/{id}]:[error:{ex}]");

                return BadRequest();
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

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[cart/all/delete]:[info:emptied cart]:[user:{userId}]");

                _cartService.AllDeleteDishesToCart(userId);

                return Ok(id);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/all/delete]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[cart/all/delete]:[error:{ex}]");

                return BadRequest();
            }
        }

    }
}