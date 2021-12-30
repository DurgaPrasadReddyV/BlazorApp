using BlazorApp.CommonInfrastructure.Identity.Models;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppUsers
    {
        public static IEnumerable<BlazorAppUser> Get()
        {
            return new List<BlazorAppUser>
            {
                new BlazorAppUser
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
