using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Не указан email")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronomic { get; set; }
    }
}
