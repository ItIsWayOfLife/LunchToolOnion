using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "MissingFirstname")]
        [Display(Name = "Firstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "MissingLastname")]
        [Display(Name = "Lastname")]
        public string Lastname { get; set; }

        [Display(Name = "Patronymic")]
        public string Patronymic { get; set; }

        [Required(ErrorMessage = "EmailNotPassword")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "EmailNotPassword")]
        [Compare("Password", ErrorMessage = "PasswordMismatch")]
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirm")]
        public string PasswordConfirm { get; set; }
    }
}
