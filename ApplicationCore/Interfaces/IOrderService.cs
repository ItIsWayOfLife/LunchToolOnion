using ApplicationCore.DTO;
using System.Collections.Generic;

namespace ApplicationCore.Interfaces
{
    public interface IOrderService
    {
        OrderDTO Create(string applicationUserId);
        IEnumerable<OrderDTO> GetOrders(string applicationUserId);
        IEnumerable<OrderDishesDTO> GetOrderDishes(string applicationUserId, int? orderId);
    }
}
