using System.Collections.Generic;

namespace Web.Models.Users
{
    public class ListUserViewModel
    {
        public IEnumerable<Web.Models.Users.UserViewModel> Users { get; set; }
    }
}
