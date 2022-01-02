using BlazorApp.CommonInfrastructure.Identity.Models;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppRoles
    {
        public static IEnumerable<BlazorAppIdentityRole> Get()
        {
            return new List<BlazorAppIdentityRole>
            {
                new BlazorAppIdentityRole
                {
                    Name = "Admin",
                    Description = "Admin Role"
                }
            };
        }
    }
}
