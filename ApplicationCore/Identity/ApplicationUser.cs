
using Microsoft.AspNetCore.Identity;

namespace ApplicationCore.Identity
{
   public class ApplicationUser : IdentityUser
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronomic { get; set; }
    }
}
