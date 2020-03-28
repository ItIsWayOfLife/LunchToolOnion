using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Dish
{
    public class ChangeMenuDishViewModel
    {
        public List<int> AddedIdDishes { get; set; }
        public List<int> DeledInDishes { get; set; }

        public ChangeMenuDishViewModel()
        {
            AddedIdDishes = new List<int>();
            DeledInDishes = new List<int>();
        }
    }
}
