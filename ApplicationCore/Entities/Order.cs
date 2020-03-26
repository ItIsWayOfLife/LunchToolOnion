using System;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{ 
    public class Order : BaseEntity
    {
        [Required]
        public DateTime DateOrder { get; set; }
        [Required]
        public string ApplicationUserId { get; set; }
    }
}
