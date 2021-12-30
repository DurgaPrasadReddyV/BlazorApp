using BlazorApp.CommonInfrastructure.Identity.Models;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppRoles
    {
        public static IEnumerable<BlazorAppRole> Get()
        {
            return new List<BlazorAppRole>
            {
                new BlazorAppRole
                {
                    Name = "Admin",
                    Description = "Admin Role"
                }
            };
        }
    }
}
