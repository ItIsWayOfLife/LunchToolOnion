using System.Collections.Generic;

namespace WebAPI.Models
{
    public class MakeMenuModel
    {
        public int MenuId { get; set; }
        public List<int> NewAddedDishes { get; set; }
        public List<int> AllSelect { get; set; }
    }
}
