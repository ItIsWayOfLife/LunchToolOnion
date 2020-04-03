using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Catalog
{
    public class AddCatalogViewModel
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "NameNotSpecified")]
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "InfoNotSpecified")]
        [Display(Name = "Info")]
        public string Info { get; set; }
    }
}
