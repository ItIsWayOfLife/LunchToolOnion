using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Menu
{
    public class EditMenuViewModel
    {
        public int ProviderId { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указана информация")]
        [Display(Name = "Мнформация")]
        public string Info { get; set; }
        [Required(ErrorMessage = "Не указана дата")]
        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}
