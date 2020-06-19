using ApplicationCore.Constants;
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
using System.Linq;
using System.Security.Claims;
using Web.Models.Order;

namespace Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;
        private readonly ILogger<OrderController> _logger;
        private readonly IStringLocalizer<SharedResource> _sharedLocalizer;

        private readonly string _path;

        public OrderController(IOrderService orderService, ICartService cartService,
            IStringLocalizer<SharedResource> sharedLocalizer,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _cartService = cartService;
                _logger = logger;
            _path = PathConstants.pathDish;
            _sharedLocalizer = sharedLocalizer;
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Order/Create");

            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
                    OrderDTO orderDTO = _orderService.Create(currentUserId);

                    _cartService.AllDeleteDishesToCart(currentUserId);

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} created order {orderDTO.Id}");

                    return RedirectToAction($"Index");
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public IActionResult Index(SortState sortOrder = SortState.DateOrderAsc)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Order/Index");

            if (User.Identity.IsAuthenticated)
            {
                try
                {
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                    IEnumerable<OrderDTO> orderDTOs = _orderService.GetOrders(currentUserId);
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDTO, OrderViewModel>()).CreateMapper();
                    var orders = mapper.Map<IEnumerable<OrderDTO>, List<OrderViewModel>>(orderDTOs);

                    ViewData["IdSort"] = sortOrder == SortState.IdAsc ? SortState.IdDesc : SortState.IdAsc;
                    ViewData["DateSort"] = sortOrder == SortState.DateOrderAsc ? SortState.DateOrderDesc : SortState.DateOrderAsc;
                    ViewData["FullPriceSort"] = sortOrder == SortState.FullPriceAsc ? SortState.FullPriceDesc : SortState.FullPriceAsc;
                    ViewData["CountDishSort"] = sortOrder == SortState.CountDishAsc ? SortState.CountDishDesc : SortState.CountDishAsc;

                    orders = sortOrder switch
                    {
                        SortState.IdDesc => orders.OrderByDescending(s => s.Id).ToList(),
                        SortState.DateOrderAsc => orders.OrderBy(s => s.DateOrder).ToList(),
                        SortState.DateOrderDesc => orders.OrderByDescending(s => s.DateOrder).ToList(),
                        SortState.FullPriceAsc => orders.OrderBy(s => s.FullPrice).ToList(),
                        SortState.FullPriceDesc => orders.OrderByDescending(s => s.FullPrice).ToList(),
                        SortState.CountDishAsc => orders.OrderBy(s => s.CountDish).ToList(),
                        SortState.CountDishDesc => orders.OrderByDescending(s => s.CountDish).ToList(),
                        _ => orders.OrderBy(s => s.Id).ToList(),
                    };

                    return View(orders);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }

        [Authorize(Roles = "employee")]
        [HttpGet]
        public IActionResult GetOrderDishes(int? orderId)
        {
            _logger.LogInformation($"{DateTime.Now.ToString()}: Processing request Order/GetOrderDishes");

            if (User.Identity.IsAuthenticated)
            {
                try
                {                   
                    string currentUserId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;

                    IEnumerable<OrderDishesDTO> orderDishesDTOs = _orderService.GetOrderDishes(currentUserId, orderId);
                    var mapper = new MapperConfiguration(cfg => cfg.CreateMap<OrderDishesDTO, OrderDishesViewModel>()).CreateMapper();
                    var orderDishes = mapper.Map<IEnumerable<OrderDishesDTO>, List<OrderDishesViewModel>>(orderDishesDTOs);

                    foreach (var oD in orderDishes)
                    {
                        oD.Path = _path + oD.Path;
                    }

                    ViewData["FullPrice"] = _orderService.GetOrders(currentUserId).Where(p=>p.Id== orderId).FirstOrDefault().FullPrice;

                    _logger.LogInformation($"{DateTime.Now.ToString()}: User {currentUserId} get order dishes {orderId}");

                    return View(orderDishes);
                }
                catch (ValidationException ex)
                {
                    ModelState.AddModelError(ex.Property, ex.Message);
                    _logger.LogError($"{DateTime.Now.ToString()}: {ex.Property}, {ex.Message}");
                }

                return BadRequest(_sharedLocalizer["BadRequest"]);
            }

            return RedirectToAction("Login", "Account");
        }
    }
}

