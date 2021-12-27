using System.Security.Claims;
using BlazorApp.Domain.Identity;

namespace BlazorApp.CommonInfrastructure.Identity.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.NameIdentifier);

    public static string? GetUserEmail(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Email);
}