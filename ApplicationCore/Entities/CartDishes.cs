using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class CartDishes : BaseEntity
    {
        [Required]
        public int Count { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
