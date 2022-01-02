using BlazorApp.Application.Common.Events;
using BlazorApp.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Application.Account.EventHandlers;

public class AccountUpdatedEventHandler : INotificationHandler<EventNotification<AccountUpdatedEvent>>
{
    private readonly ILogger<AccountUpdatedEventHandler> _logger;

    public AccountUpdatedEventHandler(ILogger<AccountUpdatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AccountUpdatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}