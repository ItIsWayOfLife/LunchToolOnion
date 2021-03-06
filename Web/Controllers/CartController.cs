﻿using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Web.Models.Cart;

namespace Web.Controllers
{
    [Authorize]
    public class CartController:Controller
    {
        private readonly ICartService _cartService;
        private readonly ILogger<CartController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        private readonly string _path;

        public CartController(ICartService cartService,
             IStringLocalizer<SharedResource> sharedLocalizer,
             ILogger<CartController> logger)
        {
            _cartService = cartService;
            _logger = logger;
            _path = PathConstants.pathDish;
            _sharedLocalizer = sharedLocalizer;
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public IActionResult Index()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Cart/Index");

            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    CartDTO cartDTO = _cartService.GetCart(currentUserId);

                    IEnumerable<CartDishesDTO> cartDishDTO = _cartService.GetCartDishes(currentUserId);
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<CartDishesDTO, CartDishesViewModel>()).CreateMapper();
                    var cartDishes = mapper.Map<IEnumerable<CartDishesDTO>, List<CartDishesViewModel>>(cartDishDTO);

                    foreach (var cD in cartDishes)
                    {
                        cD.Path = _path + cD.Path;
                    }

                 ViewData["FullPrice"] =  _cartService.FullPriceCart(currentUserId);

                    return View(cartDishes);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property} {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login","Account");
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public IActionResult Delete(int? cartDishId)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _cartService.DeleteCartDish(cartDishId, currentUserId);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} deleted cartDish {cartDishId}");

                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property} {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public IActionResult Add(int? dishId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Cart/Add");

            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _cartService.AddDishToCart(dishId, currentUserId);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} add dish {dishId}");

                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property} {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public IActionResult DeleteAll()
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    _cartService.AllDeleteDishesToCart(currentUserId);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} emptied the cart");

                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property} {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }
            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "employee")]
        [HttpPost]
        public IActionResult Update(int? dishCartId, int count)
        {
            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                _cartService.UpdateCountDishInCart(currentUserId, dishCartId, count);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} updated count dishCart {dishCartId}");

                    return RedirectToAction("Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property} {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }       
    }
}
