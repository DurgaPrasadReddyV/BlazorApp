using System.Security.Claims;
using BlazorApp.Domain.Identity;
using BlazorApp.CommonInfrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Extensions;

public static class ClaimsExtension
{
    public static async Task<IdentityResult> AddPermissionClaimAsync(this RoleManager<BlazorAppRole> roleManager, BlazorAppRole role, string permission)
    {
        var allClaims = await roleManager.GetClaimsAsync(role);
        if (!allClaims.Any(a => a.Type == ClaimConstants.Permission && a.Value == permission))
        {
            return await roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, permission));
        }

        return IdentityResult.Failed();
    }
}