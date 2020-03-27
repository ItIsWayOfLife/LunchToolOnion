using System;

namespace ApplicationCore.DTO
{
   public class СatalogDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public int ProviderId { get; set; }
    }
}
