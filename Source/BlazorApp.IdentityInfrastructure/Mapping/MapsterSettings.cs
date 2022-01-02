using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Shared.Identity;
using Mapster;

namespace BlazorApp.CommonInfrastructure.Mapping;

public class MapsterSettings
{
    public static void Configure()
    {
        // This is used in UserService.GetPermissionsAsync
        TypeAdapterConfig<BlazorAppIdentityRoleClaim, PermissionDto>.NewConfig().Map(dest => dest.Permission, src => src.ClaimValue);

    }
}