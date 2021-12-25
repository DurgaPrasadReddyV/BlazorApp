using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Auditing;

namespace BlazorApp.Application.Auditing;

public interface IAuditService : ITransientService
{
    Task<IResult<IEnumerable<AuditResponse>>> GetUserTrailsAsync(Guid userId);
}