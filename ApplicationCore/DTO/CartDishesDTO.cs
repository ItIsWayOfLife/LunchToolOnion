
namespace ApplicationCore.DTO
{
   public class CartDishesDTO: BaseEntityDTO
    {
        public int Count { get; set; }
        public int CartId { get; set; }
        public int DishId { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public double Weight { get; set; }
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
