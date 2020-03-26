using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApplicationCore.Entities
{
    public class Provider : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime TimeWorkWith { get; set; }
        [Required]
        public DateTime TimeWorkTo { get; set; }
        [Required]
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        [Required]
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
        public ICollection<Menu> Menus { get; set; }
    }
}
