using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Web.Models.Dish
{
    public class ListDishViewModel
    {
        public int CatalogId { get; set; }
      public List<DishViewModel> Dishes { get; set; }
        public SelectList SearchSelection { get; set; }
        public string SeacrhString { get; set; }
        public string SearchSelectionString { get; set; }
    }
}
