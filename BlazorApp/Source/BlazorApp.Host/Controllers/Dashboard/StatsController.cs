using BlazorApp.Application.Dashboard;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace BlazorApp.Host.Controllers.Dashboard;

[ApiConventionType(typeof(FSHApiConventions))]
public class StatsController : BaseController
{
    private readonly IStatsService _service;

    public StatsController(IStatsService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<Result<StatsDto>>> GetAsync()
    {
        var stats = await _service.GetDataAsync();
        return Ok(stats);
    }
}