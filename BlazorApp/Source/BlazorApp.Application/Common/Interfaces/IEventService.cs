using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Application.Common.Interfaces;

public interface IEventService : ITransientService
{
    Task PublishAsync(DomainEvent domainEvent);
}