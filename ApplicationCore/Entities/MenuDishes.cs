using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.Entities
{
   public class MenuDishes : BaseEntity
    {
        public int? MenuId { get; set; }
        public Menu Menu { get; set; }
        public int? DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
