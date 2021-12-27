using Microsoft.AspNetCore.Authorization;

namespace BlazorApp.CommonInfrastructure.Identity.Permissions;

public class MustHavePermission : AuthorizeAttribute
{
    public MustHavePermission(string permission)
    {
        Policy = permission;
    }
}