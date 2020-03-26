using ApplicationCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Identity
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
    {
        // ToDo add migration
        public IdentityContext(DbContextOptions<IdentityContext> options)
           : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
