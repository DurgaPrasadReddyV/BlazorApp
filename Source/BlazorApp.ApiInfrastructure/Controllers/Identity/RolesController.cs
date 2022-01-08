using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Identity;
using BlazorApp.CommonInfrastructure.Identity.Permissions;
using BlazorApp.Shared.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Host.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
[ApiVersionNeutral]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RolesController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpGet("all")]
    [MustHavePermission(Permissions.Roles.ListAll)]
    public async Task<ActionResult<Result<List<RoleDto>>>> GetListAsync()
    {
        var roles = await _roleService.GetListAsync();
        return Ok(roles);
    }

    [HttpGet("{id}")]
    [MustHavePermission(Permissions.Roles.View)]
    public async Task<ActionResult<Result<RoleDto>>> GetByIdAsync(string id)
    {
        var roles = await _roleService.GetByIdAsync(id);
        return Ok(roles);
    }

    [HttpGet("{id}/permissions")]
    public async Task<ActionResult<Result<List<PermissionDto>>>> GetPermissionsAsync(string id)
    {
        var roles = await _roleService.GetPermissionsAsync(id);
        return Ok(roles);
    }

    [HttpPut("{id}/permissions")]
    public async Task<ActionResult<Result<string>>> UpdatePermissionsAsync(string id, List<UpdatePermissionsRequest> request)
    {
        var roles = await _roleService.UpdatePermissionsAsync(id, request);
        return Ok(roles);
    }

    [HttpPost]
    [MustHavePermission(Permissions.Roles.Register)]
    public async Task<ActionResult<Result<string>>> RegisterRoleAsync(RoleRequest request)
    {
        var response = await _roleService.RegisterRoleAsync(request);
        return Ok(response);
    }

    [HttpDelete("{id}")]
    [MustHavePermission(Permissions.Roles.Remove)]
    public async Task<ActionResult<Result<string>>> DeleteAsync(string id)
    {
        var response = await _roleService.DeleteAsync(id);
        return Ok(response);
    }
}