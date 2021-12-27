using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Shared.Notifications;
using Microsoft.AspNetCore.SignalR;

namespace BlazorApp.CommonInfrastructure.Notifications;

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _notificationHubContext;

    public NotificationService(IHubContext<NotificationHub> notificationHubContext)
    {
        _notificationHubContext = notificationHubContext;
    }

    public async Task BroadcastMessageAsync(INotificationMessage notification)
    {
        await _notificationHubContext.Clients.All.SendAsync(notification.MessageType, notification);
    }

    public async Task BroadcastExceptMessageAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds)
    {
        await _notificationHubContext.Clients.AllExcept(excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageAsync(INotificationMessage notification)
    {
        await _notificationHubContext.Clients.Group($"Group").SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageExceptAsync(INotificationMessage notification, IEnumerable<string> excludedConnectionIds)
    {
        await _notificationHubContext.Clients.GroupExcept($"Group", excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupAsync(INotificationMessage notification, string group)
    {
        await _notificationHubContext.Clients.Group(group).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupsAsync(INotificationMessage notification, IEnumerable<string> groupNames)
    {
        await _notificationHubContext.Clients.Groups(groupNames).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToGroupExceptAsync(INotificationMessage notification, string group, IEnumerable<string> excludedConnectionIds)
    {
        await _notificationHubContext.Clients.GroupExcept(group, excludedConnectionIds).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToUserAsync(string userId, INotificationMessage notification)
    {
        await _notificationHubContext.Clients.User(userId).SendAsync(notification.MessageType, notification);
    }

    public async Task SendMessageToUsersAsync(IEnumerable<string> userIds, INotificationMessage notification)
    {
        await _notificationHubContext.Clients.Users(userIds).SendAsync(notification.MessageType, notification);
    }
}