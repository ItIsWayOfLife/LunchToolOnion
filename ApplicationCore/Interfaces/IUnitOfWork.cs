using ApplicationCore.Entities;
using System;

namespace ApplicationCore.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Provider> Provider { get; }
        IRepository<Catalog> Catalog { get; }
        IRepository<Dish> Dish { get; }
        IRepository<CartDishes> CartDishes { get; }
        IRepository<Cart> Cart { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDishes> OrderDishes { get; }
        void Save();
    }
}
