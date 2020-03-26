using System;

namespace ApplicationCore.DTO
{
  public  class OrderDTO : BaseEntityDTO
    {
        public DateTime DateOrder { get; set; }
        public decimal FullPrice { get; set; }
        public int CountDish { get; set; }
    }
}
