using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Application.Common.Interfaces;

public interface IEventService
{
    Task PublishAsync(DomainEvent domainEvent);
}