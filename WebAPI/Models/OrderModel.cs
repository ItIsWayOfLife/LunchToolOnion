using System;

namespace WebAPI.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime DateOrder { get; set; }
        public decimal FullPrice { get; set; }
        public int CountDish { get; set; }
    }
}
