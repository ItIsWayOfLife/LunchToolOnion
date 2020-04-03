using System;
using System.ComponentModel.DataAnnotations;

namespace Web.Models.Menu
{
    public class AddMenuViewModel
    {
        public int ProviderId { get; set; }
        [Required(ErrorMessage = "InfoNotSpecified")]
        [Display(Name = "Info")]
        public string Info { get; set; }
        [Required(ErrorMessage = "DateNotSpecified")]
        [Display(Name = "Date")]
        public DateTime Date { get; set; }
    }
}
