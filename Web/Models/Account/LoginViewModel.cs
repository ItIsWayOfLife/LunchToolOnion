using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]

        public string Email { get; set; }

        [Required(ErrorMessage = "PasswordNotSpecified")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
