using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models.Dish
{
    public class DishModel
    {
        public int Id { get; set; }
        public int CatalogId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Path { get; set; }
        public bool AddMenu { get; set; }
    }
}

