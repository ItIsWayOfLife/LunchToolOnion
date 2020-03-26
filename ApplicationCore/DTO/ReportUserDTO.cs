using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore.DTO
{
   public class ReportUserDTO : BaseEntityDTO
    {
        public DateTime DateOrder { get; set; }
        public decimal FullPrice { get; set; }
        public int CountOrderDishes { get; set; }
    }
}
