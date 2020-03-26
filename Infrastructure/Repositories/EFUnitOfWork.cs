﻿using ApplicationCore.Entities;
using ApplicationCore.Interfaces;
using Infrastructure.Data;
using System;

namespace Infrastructure.Repositories
{
    public class EFUnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _applicationContext;

        private CartDishesRepository _cartDishesRepository;
        private CartRepository _cartRepository;
        private DishRepository _dishRepository;
        private MenuRepository _menuRepository;
        private OrderDishesRepository _orderDishesRepository;
        private OrderRepository _orderRepository;
        private ProviderRepository _providerRepository;

        public EFUnitOfWork(ApplicationContext applicationContext)
        {
            _applicationContext = applicationContext;
        }

        public IRepository<Provider> Provider
        {
            get
            {
                if (_providerRepository == null)
                {
                    _providerRepository = new ProviderRepository(_applicationContext);
                }
                return _providerRepository;
            }
        }

        public IRepository<Menu> Menu
        {
            get
            {
                if (_menuRepository == null)
                {
                    _menuRepository = new MenuRepository(_applicationContext);
                }
                return _menuRepository;
            }
        }

        public IRepository<CartDishes> CartDishes
        {
            get
            {
                if (_cartDishesRepository == null)
                {
                    _cartDishesRepository = new CartDishesRepository(_applicationContext);
                }
                return _cartDishesRepository;
            }
        }

        public IRepository<Cart> Cart
        {
            get
            {
                if (_cartRepository == null)
                {
                    _cartRepository = new CartRepository(_applicationContext);
                }
                return _cartRepository;
            }
        }

        public IRepository<Order> Orders
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new OrderRepository(_applicationContext);
                }
                return _orderRepository;
            }
        }

        public IRepository<OrderDishes> OrderDishes
        {
            get
            {
                if (_orderDishesRepository == null)
                {
                    _orderDishesRepository = new OrderDishesRepository(_applicationContext);
                }
                return _orderDishesRepository;
            }
        }

        public IRepository<Dish> Dish
        {
            get
            {
                if (_dishRepository == null)
                {
                    _dishRepository = new DishRepository(_applicationContext);
                }
                return _dishRepository;
            }
        }

        private bool disposed = false;

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _applicationContext.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _applicationContext.SaveChanges();
        }
    }
}
