using Microsoft.AspNetCore.Identity;

namespace BlazorApp.Infrastructure.Identity.Models;

public class ApplicationRoleClaim : IdentityRoleClaim<string>
{
    public string? Description { get; set; }
    public string? Group { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedOn { get; set; }

    public ApplicationRoleClaim()
    {
    }

    public ApplicationRoleClaim(string? roleClaimDescription = null, string? roleClaimGroup = null)
    {
        Description = roleClaimDescription;
        Group = roleClaimGroup;
    }
}