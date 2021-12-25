using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Dashboard;

namespace BlazorApp.Application.Dashboard;

public interface IStatsService : ITransientService
{
    Task<IResult<StatsDto>> GetDataAsync();
}