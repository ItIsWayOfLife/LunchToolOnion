using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDishes> CartDishes { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDishes> OrderDishes { get; set; }
        public DbSet<Provider> Providers { get; set; }
    }
}
