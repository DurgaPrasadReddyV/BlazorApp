using Microsoft.AspNetCore.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Models;

public class BlazorAppRole : IdentityRole
{
    public string? Description { get; set; }

    public BlazorAppRole()
    {
    }

    public BlazorAppRole(string roleName, string? description = null)
    : base(roleName)
    {
        Description = description;
        NormalizedName = roleName.ToUpper();
    }
}