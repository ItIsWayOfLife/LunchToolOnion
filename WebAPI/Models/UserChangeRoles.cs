using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class UserChangeRoles
    {
        public string Id { get; set; }
        public List<string> Roles { get; set; }

        public UserChangeRoles()
        {
            Roles = new List<string>();
        }

    }
}
