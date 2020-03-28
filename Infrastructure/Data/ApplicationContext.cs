using ApplicationCore.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
        //    {
        //        relationship.DeleteBehavior = DeleteBehavior.SetNull;
        //    }
        //}


        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<MenuDishes>()
        //        .HasOne(b => b.Dish)
        //        .WithMany(a => a.)
        //        .OnDelete(DeleteBehavior.SetNull);
        //}


        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartDishes> CartDishes { get; set; }
        public DbSet<Dish> Dishes { get; set; }
        public DbSet<Catalog> Catalogs { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDishes> OrderDishes { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Menu> Menus { get; set; }
        public DbSet<MenuDishes>  MenuDishes { get; set; }
    }
}
