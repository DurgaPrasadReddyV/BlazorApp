using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Dashboard;

namespace BlazorApp.Application.Dashboard;

public interface IStatsService
{
    Task<IResult<StatsDto>> GetDataAsync();
}