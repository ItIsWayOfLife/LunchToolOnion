using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Catalog
{
    public class AddCatalogViewModel
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "Не указано название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана информация")]
        [Display(Name = "Информация")]
        public string Info { get; set; }
    }
}
