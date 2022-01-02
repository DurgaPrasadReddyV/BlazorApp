using Microsoft.AspNetCore.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Models;

public class BlazorAppIdentityRoleClaim : IdentityRoleClaim<string>
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public BlazorAppIdentityRoleClaim()
    {
    }

    public BlazorAppIdentityRoleClaim(string? roleClaimDescription = null, string? roleClaimGroup = null)
    {
        Description = roleClaimDescription;
        Group = roleClaimGroup;
    }
}