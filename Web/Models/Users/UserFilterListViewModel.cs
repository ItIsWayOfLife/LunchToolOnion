using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Models.Users
{
    public class UserFilterListViewModel
    {
        public SelectList SearchSelection { get; set; }
        public ListUserViewModel ListUsers { get; set; }
        public string SeacrhString { get; set; }
        public string SearchSelectionString { get; set; }
    }
}
