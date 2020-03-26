using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Menu
{
    public class AddMenuViewModel
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "Не указано название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана информация")]
        [Display(Name = "Мнформация")]
        public string Info { get; set; }
        [Required(ErrorMessage = "Не указана дата")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}
