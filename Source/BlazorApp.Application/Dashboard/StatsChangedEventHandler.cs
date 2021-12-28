using BlazorApp.Application.Common.Events;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Domain.Dashboard;
using BlazorApp.Shared.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.Application.Dashboard;

public class StatsChangedEventHandler : INotificationHandler<EventNotification<StatsChangedEvent>>
{
    private readonly ILogger<StatsChangedEventHandler> _logger;
    private readonly INotificationService _notificationService;

    public StatsChangedEventHandler(ILogger<StatsChangedEventHandler> logger, INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task Handle(EventNotification<StatsChangedEvent> notification, CancellationToken cancellationToken)
    {
        await _notificationService.SendMessageAsync(new StatsChangedNotification());
        _logger.LogInformation("{event} Triggered", notification.DomainEvent.GetType().Name);
    }
}