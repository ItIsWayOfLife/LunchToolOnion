using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "PasswordNotSpecified")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronomic { get; set; }
    }
}
