using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class ProfileViewModel
    {
        public string Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "Не указан email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
