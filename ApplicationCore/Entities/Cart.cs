using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
   public class Cart : BaseEntity
    {
        [Required]
        public string ApplicationUserId { get; set; }
    }
}
