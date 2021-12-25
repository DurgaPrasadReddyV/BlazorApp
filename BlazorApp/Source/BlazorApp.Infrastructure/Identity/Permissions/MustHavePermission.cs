using Microsoft.AspNetCore.Authorization;

namespace BlazorApp.Infrastructure.Identity.Permissions;

public class MustHavePermission : AuthorizeAttribute
{
    public MustHavePermission(string permission)
    {
        Policy = permission;
    }
}