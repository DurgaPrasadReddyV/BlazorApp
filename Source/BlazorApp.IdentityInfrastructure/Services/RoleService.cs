using System.Net;
using BlazorApp.Application.Identity.Exceptions;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Identity;
using BlazorApp.CommonInfrastructure.Common.Extensions;
using BlazorApp.CommonInfrastructure.Identity.Extensions;
using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using BlazorApp.Shared.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace BlazorApp.CommonInfrastructure.Identity.Services;

public class RoleService : IRoleService
{
    private readonly ICurrentUser _currentUser;
    private readonly RoleManager<BlazorAppIdentityRole> _roleManager;
    private readonly UserManager<BlazorAppIdentityUser> _userManager;
    private readonly IdentityDbContext _context;
    private readonly IRoleClaimsService _roleClaimService;

    public RoleService(RoleManager<BlazorAppIdentityRole> roleManager, UserManager<BlazorAppIdentityUser> userManager, IdentityDbContext context, ICurrentUser currentUser, IRoleClaimsService roleClaimService)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _context = context;
        _currentUser = currentUser;
        _roleClaimService = roleClaimService;
    }

    public async Task<Result<string>> DeleteAsync(string id)
    {
        var existingRole = await _roleManager.FindByIdAsync(id);
        if (existingRole == null)
        {
            throw new IdentityException("Role Not Found", statusCode: HttpStatusCode.NotFound);
        }

        if (DefaultRoles.Contains(existingRole.Name))
        {
            return await Result<string>.FailAsync(string.Format("Not allowed to delete {0} Role.", existingRole.Name));
        }

        bool roleIsNotUsed = true;
        var allUsers = await _userManager.Users.ToListAsync();
        foreach (var user in allUsers)
        {
            if (await _userManager.IsInRoleAsync(user, existingRole.Name))
            {
                roleIsNotUsed = false;
            }
        }

        if (roleIsNotUsed)
        {
            await _roleManager.DeleteAsync(existingRole);
            return await Result<string>.SuccessAsync(existingRole.Id, string.Format("Role {0} Deleted.", existingRole.Name));
        }
        else
        {
            return await Result<string>.FailAsync(string.Format("Not allowed to delete {0} Role as it is being used.", existingRole.Name));
        }
    }

    public async Task<Result<RoleDto>> GetByIdAsync(string id)
    {
        var role = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id == id);
        if (role is null)
        {
            throw new IdentityException("Role Not Found", statusCode: HttpStatusCode.NotFound);
        }

        var rolesResponse = role.Adapt<RoleDto>();
        rolesResponse.IsDefault = DefaultRoles.Contains(role.Name);
        return await Result<RoleDto>.SuccessAsync(rolesResponse);
    }

    public async Task<int> GetCountAsync()
    {
        return await _roleManager.Roles.CountAsync();
    }

    public async Task<Result<List<RoleDto>>> GetListAsync()
    {
        var roles = await _roleManager.Roles.ToListAsync();
        var rolesResponse = roles.Adapt<List<RoleDto>>();
        foreach (var role in rolesResponse)
        {
            role.IsDefault = DefaultRoles.Contains(role.Name!);
        }
        return await Result<List<RoleDto>>.SuccessAsync(rolesResponse);
    }

    public async Task<Result<List<PermissionDto>>> GetPermissionsAsync(string id)
    {
        var permissions = await _context.RoleClaims.Where(a => a.RoleId == id && a.ClaimType == ClaimTypes.Permission).ToListAsync();
        var permissionResponse = permissions.Adapt<List<PermissionDto>>();
        return await Result<List<PermissionDto>>.SuccessAsync(permissionResponse);
    }

    public async Task<Result<List<RoleDto>>> GetUserRolesAsync(string userId)
    {
        var userRoles = await _context.UserRoles.Where(a => a.UserId == userId).Select(a => a.RoleId).ToListAsync();
        var roles = await _roleManager.Roles.Where(a => userRoles.Contains(a.Id)).ToListAsync();

        var rolesResponse = roles.Adapt<List<RoleDto>>();
        foreach (var role in rolesResponse)
        {
            role.IsDefault = DefaultRoles.Contains(role.Name);
        }
        return await Result<List<RoleDto>>.SuccessAsync(rolesResponse);
    }

    public async Task<bool> ExistsAsync(string roleName, string? excludeId)
    {
        return await _roleManager.FindByNameAsync(roleName)
            is BlazorAppIdentityRole existingRole
            && existingRole.Id != excludeId;
    }

    public async Task<Result<string>> RegisterRoleAsync(RoleRequest request)
    {
        if (string.IsNullOrEmpty(request.Id))
        {
            var newRole = new BlazorAppIdentityRole(request.Name, request.Description);
            var response = await _roleManager.CreateAsync(newRole);
            await _context.SaveChangesAsync();
            if (response.Succeeded)
            {
                return await Result<string>.SuccessAsync(newRole.Id, string.Format("Role {0} Created.", request.Name));
            }
            else
            {
                return await Result<string>.FailAsync(response.Errors.Select(e => e.Description.ToString()).ToList());
            }
        }
        else
        {
            var existingRole = await _roleManager.FindByIdAsync(request.Id);
            if (existingRole == null)
            {
                return await Result<string>.FailAsync("Role does not exist.");
            }

            if (DefaultRoles.Contains(existingRole.Name))
            {
                return await Result<string>.SuccessAsync(string.Format("Not allowed to modify {0} Role.", existingRole.Name));
            }

            existingRole.Name = request.Name;
            existingRole.NormalizedName = request.Name.ToUpper();
            existingRole.Description = request.Description;
            var result = await _roleManager.UpdateAsync(existingRole);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(existingRole.Id, string.Format("Role {0} Updated.", existingRole.Name));
            }
            else
            {
                return await Result<string>.FailAsync(result.Errors.Select(e => e.Description.ToString()).ToList());
            }
        }
    }

    public async Task<Result<string>> UpdatePermissionsAsync(string roleId, List<UpdatePermissionsRequest> request)
    {
        try
        {
            var errors = new List<string>();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return await Result<string>.FailAsync("Role does not exist.");
            }

            if (role.Name == Domain.Identity.DefaultRoles.Admin)
            {
                var currentUser = await _userManager.Users.SingleAsync(x => x.Id == _currentUser.GetUserId().ToString());
                if (await _userManager.IsInRoleAsync(currentUser, Domain.Identity.DefaultRoles.Admin))
                {
                    return await Result<string>.FailAsync("Not allowed to modify Permissions for this Role.");
                }
            }

            var selectedPermissions = request.Where(a => a.Enabled).ToList();
            if (role.Name == Domain.Identity.DefaultRoles.Admin)
            {
                if (!selectedPermissions.Any(x => x.Permission == Permissions.Roles.View)
                   || !selectedPermissions.Any(x => x.Permission == Permissions.RoleClaims.View)
                   || !selectedPermissions.Any(x => x.Permission == Permissions.RoleClaims.Edit))
                {
                    return await Result<string>.FailAsync(string.Format("Not allowed to deselect {0} or {1} or {2} for this Role.",
                        Permissions.Roles.View,
                        Permissions.RoleClaims.View,
                        Permissions.RoleClaims.Edit));
                }
            }

            var permissions = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in permissions.Where(p => request.Any(a => a.Permission == p.Value)))
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }

            foreach (var permission in selectedPermissions)
            {
                if (!string.IsNullOrEmpty(permission.Permission))
                {
                    var addResult = await _roleManager.AddPermissionClaimAsync(role, permission.Permission);
                    if (!addResult.Succeeded)
                    {
                        errors.AddRange(addResult.Errors.Select(e => e.Description.ToString()));
                    }
                }
            }

            var addedPermissions = await _roleClaimService.GetAllByRoleIdAsync(role.Id);
            if (addedPermissions.Succeeded)
            {
                foreach (var permission in selectedPermissions)
                {
                    var addedPermission = addedPermissions.Data?.SingleOrDefault(x => x.Type == ClaimTypes.Permission && x.Value == permission.Permission);
                    if (addedPermission != null)
                    {
                        var newPermission = addedPermission.Adapt<RoleClaimRequest>();
                        var saveResult = await _roleClaimService.SaveAsync(newPermission);
                        if (!saveResult.Succeeded && saveResult.Messages is not null)
                        {
                            errors.AddRange(saveResult.Messages);
                        }
                    }
                }
            }
            else if (addedPermissions.Messages is not null)
            {
                errors.AddRange(addedPermissions.Messages);
            }

            if (errors.Count > 0)
            {
                return await Result<string>.FailAsync(errors);
            }

            return await Result<string>.SuccessAsync("Permissions Updated.");
        }
        catch (Exception ex)
        {
            return await Result<string>.FailAsync(ex.Message);
        }
    }

    internal static List<string> DefaultRoles =>
        typeof(DefaultRoles).GetAllPublicConstantValues<string>();
}