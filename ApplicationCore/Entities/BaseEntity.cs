using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
   public class BaseEntity
    {
        [Required]
        public int Id { get; private set; }
    }
}
