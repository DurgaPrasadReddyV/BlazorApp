using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Domain.Identity;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppRoleClaims
    {
        public static IEnumerable<BlazorAppIdentityRoleClaim> Get()
        {
            return new List<BlazorAppIdentityRoleClaim>
            {
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Identity.Register
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Roles.Register
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Roles.View
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Roles.Update
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Roles.Remove
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Roles.ListAll
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.RoleClaims.Search
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.RoleClaims.View
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.RoleClaims.Edit
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.RoleClaims.Create
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.RoleClaims.Delete
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.Export
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.Search
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.View
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.Edit
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.Create
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Users.Delete
                },
                new BlazorAppIdentityRoleClaim
                {
                    ClaimType = ClaimTypes.Permission,
                    ClaimValue = Permissions.Dashboard.View
                }
            };
        }
    }















}
