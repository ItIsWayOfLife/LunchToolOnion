using ApplicationCore.Constants;
using ApplicationCore.DTO;
using ApplicationCore.Identity;
using ApplicationCore.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly IUserHelper _userHelper;

        private readonly string _path;

        public OrderController(IOrderService orderService,
            ICartService cartService,
             IUserHelper userHelper)
        {
            _orderService = orderService;
            _cartService = cartService;
            _userHelper = userHelper;

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

                IEnumerable<OrderDTO> orderDTOs = _orderService.GetOrders(userId);
                var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderModel>()).CreateMapper();
                var orders = mapper.Map<IEnumerable<OrderDTO>, List<OrderModel>>(orderDTOs);

                return new ObjectResult(orders);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
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

                return new ObjectResult(orderDishes);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
