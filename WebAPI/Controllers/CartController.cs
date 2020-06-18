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
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin, employee")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly PathConstants _pathConstants;
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly string _path;

        public CartController(ICartService cartService,
               UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;

            _pathConstants = new PathConstants();
            _path = _pathConstants.APIURL + _pathConstants.pathForAPI;
        }

        [HttpGet, Route("dishes")]
        public IActionResult GetDishes()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = null;

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                     user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    CartDTO cartDTO = _cartService.GetCart(user.Id);

                    IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(user.Id);
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
            }else
            {
                return BadRequest("No Authenticated");
            }
        }


        [HttpGet, Route("fullprice")]
        public IActionResult GetFullPrice()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = null;

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    string fullPrice = _cartService.FullPriceCart(user.Id).ToString();

                    return new ObjectResult(fullPrice);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return BadRequest("No Authenticated");
            }
        }

        [HttpPut, Route("updatecart")]
        public IActionResult UpdateCartDishes(List<CartDishesUpdateModel> models)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    ApplicationUser user = null;

                    string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                    user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);

                    if (user == null)
                    {
                        return NotFound();
                    }

                    foreach (var m in models)
                    {
                        _cartService.UpdateCountDishInCart(user.Id, m.Id, m.Count);
                    }

                    return  Ok();
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }
            }
            else
            {
                return BadRequest("No Authenticated");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                ApplicationUser user = null;

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);
                _cartService.DeleteCartDish(id, user.Id);

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
                ApplicationUser user = null;

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);
                _cartService.AddDishToCart(id, user.Id);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete, Route("deleteall")]
        public IActionResult DeleteAllDishesInCart(int id)
        {
            try
            {
                ApplicationUser user = null;

                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                user = _userManager.Users.FirstOrDefault(p => p.Email == currentEmail);
                _cartService.AllDeleteDishesToCart(user.Id);

                return Ok(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
