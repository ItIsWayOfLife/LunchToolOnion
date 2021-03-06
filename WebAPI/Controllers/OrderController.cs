﻿using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Exceptions;
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
    [Authorize(Roles = "admin, employee")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserHelper _userHelper;
        private readonly ILogger<OrderController> _logger;

        private readonly string _path;

        public OrderController(IOrderService orderService,
            ICartService cartService,
             IUserHelper userHelper,
             ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userHelper = userHelper;
            _logger = logger;

            _path = PathConstants.APIURL + PathConstants.pathForAPI;
        }

        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                var orderDTOs = _orderService.GetOrders(userId).OrderByDescending(p => p.DateOrder).ToList();
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderModel>()).CreateMapper();
                var orders = mapper.Map<IEnumerable<OrderDTO>, List<OrderModel>>(orderDTOs);

                for (int i = 0; i > orderDTOs.Count(); i++)
                {
                    orders[i].DateOrder = orderDTOs[i].DateOrder.ToString();
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[order/get]:[info:get orders]:[user:{userId}]");

                return new ObjectResult(orders);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/get]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/get]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetDishes(int id)
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                IEnumerable<OrderDishesDTO> orderDishesDTOs = _orderService.GetOrderDishes(userId, id);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDishesDTO, OrderDishesModel>()).CreateMapper();
                var orderDishes = mapper.Map<IEnumerable<OrderDishesDTO>, List<OrderDishesModel>>(orderDishesDTOs);

                foreach (var oD in orderDishes)
                {
                    oD.Path = _path + oD.Path;
                }

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[order/get/{id}]:[info:get dishes in order {id}]:[user:{userId}]");

                return new ObjectResult(orderDishes);
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/get/{id}]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/get/{id}]:[error:{ex}]");

                return BadRequest();
            }
        }

        [HttpPost]
        public IActionResult Post()
        {
            try
            {
                string currentEmail = this.User.FindFirst(ClaimTypes.Name).Value;
                string userId = _userHelper.GetUserId(currentEmail);

                if (userId == null)
                {
                    return NotFound("User not found");
                }

                OrderDTO orderDTO = _orderService.Create(userId);
                _cartService.AllDeleteDishesToCart(userId);

                _logger.LogInformation($"[{DateTime.Now.ToString()}]:[order/post]:[info:create order]:[user:{userId}]");

                return Ok();
            }
            catch (ValidationException ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/post]:[error:{ex.Property}, {ex.Message}]");

                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError($"[{DateTime.Now.ToString()}]:[order/post]:[error:{ex}]");

                return BadRequest();
            }
        }

    }
}
