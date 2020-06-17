using System;

namespace ApplicationCore.DTO
{
   public class CatalogDTO : BaseEntityDTO
    {
        public string Name { get; set; }
        public string Info { get; set; }
        public int ProviderId { get; set; }
    }
}
