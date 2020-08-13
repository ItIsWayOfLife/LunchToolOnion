using ApplicationCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace WebAPI.Controllers.Identity
{
    [Authorize]
    public class UserHelper : IUserHelper
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserHelper(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public string GetUserId(string email)
        {
            try
            {
                ApplicationUser user = null;
                user = _userManager.Users.FirstOrDefault(p => p.Email == email);

                if (user == null)
                {
                    return null;
                }

                return user.Id;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
