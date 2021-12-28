using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace BlazorApp.CommonInfrastructure.Notifications;

[Authorize]
public class NotificationHub : Hub
{
    private readonly ILogger<NotificationHub> _logger;

    public NotificationHub(ILogger<NotificationHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"Group");
        await base.OnConnectedAsync();
        _logger.LogInformation($"A client connected to NotificationHub: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Group");
        await base.OnDisconnectedAsync(exception);
        _logger.LogInformation($"A client disconnected from NotificationHub: {Context.ConnectionId}");
    }
}