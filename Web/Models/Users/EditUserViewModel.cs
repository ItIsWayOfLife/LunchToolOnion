using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronomic { get; set; }
    }
}
