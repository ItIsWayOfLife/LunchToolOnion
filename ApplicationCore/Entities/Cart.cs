using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
   public class Cart : BaseEntity
    {
        public string ApplicationUserId { get; set; }
    }
}
