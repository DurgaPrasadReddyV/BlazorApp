using BlazorApp.Application.Common.Events;
using BlazorApp.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Application.Account.EventHandlers;

public class AccountCreatedEventHandler : INotificationHandler<EventNotification<AccountCreatedEvent>>
{
    private readonly ILogger<AccountCreatedEventHandler> _logger;

    public AccountCreatedEventHandler(ILogger<AccountCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AccountCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}