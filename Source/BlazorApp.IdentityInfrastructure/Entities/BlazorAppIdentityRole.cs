using Microsoft.AspNetCore.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Models;

public class BlazorAppIdentityRole : IdentityRole
{
    public string? Description { get; set; }

    public BlazorAppIdentityRole()
    {
    }

    public BlazorAppIdentityRole(string roleName, string? description = null)
    : base(roleName)
    {
        Description = description;
        NormalizedName = roleName.ToUpper();
    }
}