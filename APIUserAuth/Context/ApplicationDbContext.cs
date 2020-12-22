using APIUserAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace APIUserAuth.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            :base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            var roleAdmin = new IdentityRole()
            {
                Id = "3092713d-c53c-4007-af04-cae19bd334b1",
                Name = "admin",
                NormalizedName = "admin"
            };

            builder.Entity<IdentityRole>().HasData(roleAdmin);

            base.OnModelCreating(builder);
        }
    }
}
