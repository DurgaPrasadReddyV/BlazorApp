using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Identity;
using BlazorApp.Shared.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Host.Controllers.Identity;

[ApiController]
[Route("api/[controller]")]
[ApiVersionNeutral]
[ApiConventionType(typeof(FSHApiConventions))]
public class RoleClaimsController : ControllerBase
{
    private readonly IRoleClaimsService _roleClaimService;

    public RoleClaimsController(IRoleClaimsService roleClaimService)
    {
        _roleClaimService = roleClaimService;
    }

    [Authorize(Policy = Permissions.RoleClaims.View)]
    [HttpGet]
    public async Task<ActionResult<Result<List<RoleClaimResponse>>>> GetAllAsync()
    {
        var roleClaims = await _roleClaimService.GetAllAsync();
        return Ok(roleClaims);
    }

    [Authorize(Policy = Permissions.RoleClaims.View)]
    [HttpGet("{roleId}")]
    public async Task<ActionResult<Result<List<RoleClaimResponse>>>> GetAllByRoleIdAsync([FromRoute] string roleId)
    {
        var response = await _roleClaimService.GetAllByRoleIdAsync(roleId);
        return Ok(response);
    }

    [Authorize(Policy = Permissions.RoleClaims.Create)]
    [HttpPost]
    public async Task<ActionResult<Result<string>>> PostAsync(RoleClaimRequest request)
    {
        var response = await _roleClaimService.SaveAsync(request);
        return Ok(response);
    }

    [Authorize(Policy = Permissions.RoleClaims.Delete)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Result<string>>> DeleteAsync(int id)
    {
        var response = await _roleClaimService.DeleteAsync(id);
        return Ok(response);
    }
}