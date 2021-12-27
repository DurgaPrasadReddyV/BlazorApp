using BlazorApp.Application.Common.Events;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Domain.Common.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.CommonInfrastructure.Common.Services;

public class EventService : IEventService
{
    private readonly ILogger<EventService> _logger;
    private readonly IPublisher _mediator;

    public EventService(ILogger<EventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task PublishAsync(DomainEvent @event)
    {
        _logger.LogInformation("Publishing Event : {event}", @event.GetType().Name);
        await _mediator.Publish(GetEventNotification(@event));
    }

    private INotification GetEventNotification(DomainEvent @event)
    {
        return (INotification)Activator.CreateInstance(
            typeof(EventNotification<>).MakeGenericType(@event.GetType()), @event)!;
    }
}