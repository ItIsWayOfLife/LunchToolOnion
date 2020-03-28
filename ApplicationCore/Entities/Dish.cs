using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Entities
{
    public class Dish : BaseEntity
    {
        public int CatalogId { get; set; }
        public Catalog Сatalog { get; set; }
        public string Name { get; set; }
        [Required]
        public string Info { get; set; }
        [Required]
        public double Weight { get; set; }
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public string Path { get; set; }
    }
}
