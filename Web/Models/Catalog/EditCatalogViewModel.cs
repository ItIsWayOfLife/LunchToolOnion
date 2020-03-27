using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Catalog
{
    public class EditCatalogViewModel
    {
        public int ProviderId { get; set; }
        public int Id { get; set; }
        [Required(ErrorMessage = "Не указано название")]
        [Display(Name = "Название")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Не указана информация")]
        [Display(Name = "Мнформация")]
        public string Info { get; set; }
    }
}
