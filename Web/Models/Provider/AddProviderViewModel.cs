using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Provider
{
    public class AddProviderViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [Display(Name = "Имя")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указан email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указано время открытия")]
        [Display(Name = "Время открытия")]
        public DateTime TimeWorkWith { get; set; }
        [Required(ErrorMessage = "Не указано время закрытия")]
        [Display(Name = "Время закрытия")]
        public DateTime TimeWorkTo { get; set; }
        public bool IsActive { get; set; }
        public string Path { get; set; }
        public string WorkingDays { get; set; }
        public bool IsFavorite { get; set; }
        public string Info { get; set; }
    }
}
