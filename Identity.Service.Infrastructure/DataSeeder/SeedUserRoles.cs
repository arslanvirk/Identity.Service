using Microsoft.EntityFrameworkCore;
using Identity.Service.Core.Entities;

namespace Identity.Service.Infrastructure.DataSeeder
{
    public static class UserRoleSeeder
    {
        public static void SeedUserRoles(this ModelBuilder builder)
        {
            builder.Entity<UserRole>().HasData(
                new UserRole { Id = new Guid("181d84e2-afb6-499b-9972-da57e1e63777"), Name = "User", NormalizedName = "USER" },
                new UserRole { Id = new Guid("181d84e2-afb6-499b-9972-da57e1e63888"), Name = "Admin", NormalizedName = "ADMIN" }
            );
        }
    }
}
