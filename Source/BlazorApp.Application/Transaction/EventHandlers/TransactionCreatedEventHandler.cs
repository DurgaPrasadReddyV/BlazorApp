using BlazorApp.Application.Common.Events;
using BlazorApp.Domain.Transaction.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Application.Transaction.EventHandlers;

public class TransactionCreatedEventHandler : INotificationHandler<EventNotification<TransactionCreatedEvent>>
{
    private readonly ILogger<TransactionCreatedEventHandler> _logger;

    public TransactionCreatedEventHandler(ILogger<TransactionCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(EventNotification<TransactionCreatedEvent> notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}