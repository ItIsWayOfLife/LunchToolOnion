using System.ComponentModel.DataAnnotations;

namespace Web.Models.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не указан email")]
        [Display(Name = "Email")]

        public string Email { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [Display(Name = "Запомнить?")]
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
