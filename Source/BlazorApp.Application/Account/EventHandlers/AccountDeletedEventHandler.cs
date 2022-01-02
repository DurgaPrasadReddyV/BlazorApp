using BlazorApp.Application.Common.Events;
using BlazorApp.Domain.Account.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Application.Account.EventHandlers;

public class AccountDeletedEventHandler : INotificationHandler<EventNotification<AccountDeletedEvent>>
{
    private readonly ILogger<AccountDeletedEventHandler> _logger;

    public AccountDeletedEventHandler(ILogger<AccountDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<AccountDeletedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}