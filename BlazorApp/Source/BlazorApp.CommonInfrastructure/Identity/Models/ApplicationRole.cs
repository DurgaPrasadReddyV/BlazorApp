using Microsoft.AspNetCore.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Models;

public class ApplicationRole : IdentityRole
{
    public string? Description { get; set; }

    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName, string? description = null)
    : base(roleName)
    {
        Description = description;
        NormalizedName = roleName.ToUpper();
    }
}