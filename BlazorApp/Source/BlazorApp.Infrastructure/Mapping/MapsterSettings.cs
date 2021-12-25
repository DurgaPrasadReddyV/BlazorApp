using BlazorApp.Infrastructure.Identity.Models;
using BlazorApp.Shared.Identity;
using Mapster;

namespace DN.WebApi.Infrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // This is used in UserService.GetPermissionsAsync
        TypeAdapterConfig<ApplicationRoleClaim, PermissionDto>.NewConfig().Map(dest => dest.Permission, src => src.ClaimValue);

    }
}