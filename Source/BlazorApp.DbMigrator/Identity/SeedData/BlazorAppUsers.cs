using BlazorApp.CommonInfrastructure.Identity.Models;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppUsers
    {
        public static IEnumerable<BlazorAppIdentityUser> Get()
        {
            return new List<BlazorAppIdentityUser>
            {
                new BlazorAppIdentityUser
                {
                    FirstName = "Administrator",
                    UserName = "admin",
                    Email = "admin@blazorapp.com",
                    EmailConfirmed = true,
                    IsActive = true
                }
            };
        }
    }
}
