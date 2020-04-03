using System.ComponentModel.DataAnnotations;

namespace Web.Models.Users
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string FLP { get; set; }

        [Required(ErrorMessage = "EmailNotSpecified")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
