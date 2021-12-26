using BlazorApp.Application.Auditing;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Auditing;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Host.Controllers.Identity;

[ApiController]
[Route("api/audit-logs")]
[ApiVersionNeutral]
[ApiConventionType(typeof(FSHApiConventions))]
public class AuditLogsController : ControllerBase
{
    private readonly ICurrentUser _user;
    private readonly IAuditService _auditService;

    public AuditLogsController(IAuditService auditService, ICurrentUser user)
    {
        _auditService = auditService;
        _user = user;
    }

    [HttpGet]
    public async Task<ActionResult<Result<List<AuditResponse>>>> GetMyLogsAsync()
    {
        return Ok(await _auditService.GetUserTrailsAsync(_user.GetUserId()));
    }
}