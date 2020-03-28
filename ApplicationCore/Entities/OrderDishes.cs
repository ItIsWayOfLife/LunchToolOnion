using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class OrderDishes : BaseEntity
    {
        [Required]
        public int Count { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int DishId { get; set; }
        public Dish Dish { get; set; }
    }
}
