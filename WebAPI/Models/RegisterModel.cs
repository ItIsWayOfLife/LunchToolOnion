
using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "EmailNotSpecified")]
        public string Email { get; set; }

        [Required(ErrorMessage = "MissingFirstname")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "MissingLastname")]
        public string Lastname { get; set; }

        public string Patronymic { get; set; }

        [Required(ErrorMessage = "EmailNotPassword")]
        public string Password { get; set; }

        [Required(ErrorMessage = "EmailNotPassword")]
        public string PasswordConfirm { get; set; }
    }
}
