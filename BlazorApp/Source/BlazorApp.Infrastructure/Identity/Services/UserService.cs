using System.Linq.Dynamic.Core;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Domain.Identity;
using BlazorApp.Infrastructure.Identity.Models;
using BlazorApp.Infrastructure.Mapping;
using BlazorApp.Infrastructure.Persistence;
using BlazorApp.Infrastructure.Persistence.Contexts;
using BlazorApp.Shared.Identity;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BlazorApp.Infrastructure.Identity.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    public async Task<PaginatedResult<UserDetailsDto>> SearchAsync(UserListFilter filter)
    {
        var filters = new Filters<ApplicationUser>();
        filters.Add(filter.IsActive.HasValue, x => x.IsActive == filter.IsActive);

        var query = _userManager.Users.ApplyFilter(filters);
        if (filter.AdvancedSearch is not null)
            query = query.AdvancedSearch(filter.AdvancedSearch);
        string? ordering = new OrderByConverter().ConvertBack(filter.OrderBy);
        query = !string.IsNullOrWhiteSpace(ordering) ? query.OrderBy(ordering) : query.OrderBy(a => a.Id);

        return await query.ToMappedPaginatedResultAsync<ApplicationUser, UserDetailsDto>(filter.PageNumber, filter.PageSize);
    }

    public async Task<Result<List<UserDetailsDto>>> GetAllAsync()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();
        var result = users.Adapt<List<UserDetailsDto>>();
        return await Result<List<UserDetailsDto>>.SuccessAsync(result);
    }

    public async Task<IResult<UserDetailsDto>> GetAsync(string userId)
    {
        var user = await _userManager.Users.AsNoTracking().Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user is null)
        {
            return await Result<UserDetailsDto>.FailAsync("User Not Found.");
        }

        var result = user.Adapt<UserDetailsDto>();
        return await Result<UserDetailsDto>.SuccessAsync(result);
    }

    public async Task<IResult<UserRolesResponse>> GetRolesAsync(string userId)
    {
        var viewModel = new List<UserRoleDto>();
        var user = await _userManager.FindByIdAsync(userId);
        var roles = await _roleManager.Roles.AsNoTracking().ToListAsync();
        foreach (var role in roles)
        {
            var userRolesViewModel = new UserRoleDto
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            userRolesViewModel.Enabled = await _userManager.IsInRoleAsync(user, role.Name);

            viewModel.Add(userRolesViewModel);
        }

        var result = new UserRolesResponse { UserRoles = viewModel };
        return await Result<UserRolesResponse>.SuccessAsync(result);
    }

    public async Task<IResult<string>> AssignRolesAsync(string userId, UserRolesRequest request)
    {
        var user = await _userManager.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
        if (user == null)
        {
            return await Result<string>.FailAsync("User Not Found.");
        }

        if (await _userManager.IsInRoleAsync(user, RoleConstants.Admin))
        {
            return await Result<string>.FailAsync("Not Allowed.");
        }

        foreach (var userRole in request.UserRoles)
        {
            // Check if Role Exists
            if (await _roleManager.FindByNameAsync(userRole.RoleName) != null)
            {
                if (userRole.Enabled)
                {
                    if (!await _userManager.IsInRoleAsync(user, userRole.RoleName))
                    {
                        await _userManager.AddToRoleAsync(user, userRole.RoleName);
                    }
                }
                else
                {
                    await _userManager.RemoveFromRoleAsync(user, userRole.RoleName);
                }
            }
        }

        return await Result<string>.SuccessAsync(userId, string.Format("User Roles Updated Successfully."));
    }

    public async Task<Result<List<PermissionDto>>> GetPermissionsAsync(string userId)
    {
        var userPermissions = new List<PermissionDto>();
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result<List<PermissionDto>>.FailAsync("User Not Found.");
        }

        var roleNames = await _userManager.GetRolesAsync(user);
        foreach (var role in _roleManager.Roles.Where(r => roleNames.Contains(r.Name)).ToList())
        {
            var permissions = await _context.RoleClaims.Where(a => a.RoleId == role.Id && a.ClaimType == ClaimConstants.Permission).ToListAsync();
            var permissionResponse = permissions.Adapt<List<PermissionDto>>();
            userPermissions.AddRange(permissionResponse);
        }

        return await Result<List<PermissionDto>>.SuccessAsync(userPermissions.Distinct().ToList());
    }

    public async Task<int> GetCountAsync()
    {
        return await _userManager.Users.AsNoTracking().CountAsync();
    }
}