using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Domain.Identity;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppRoleClaims
    {
        public static IEnumerable<BlazorAppRoleClaim> Get()
        {
            return new List<BlazorAppRoleClaim>
            {
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Identity.Register
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Roles.Register
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Roles.View
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Roles.Update
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Roles.Remove
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Roles.ListAll
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.RoleClaims.Search
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.RoleClaims.View
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.RoleClaims.Edit
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.RoleClaims.Create
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.RoleClaims.Delete
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.Export
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.Search
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.View
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.Edit
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.Create
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = PermissionConstants.Users.Delete
                },
                new BlazorAppRoleClaim
                {
                    ClaimType = ClaimConstants.Permission,
                    ClaimValue = BlazorApp.Domain.Dashboard.PermissionConstants.Dashboard.View
                }
            };
        }
    }
}
