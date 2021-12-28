namespace BlazorApp.Client.Infrastructure.Notifications;

public record ConnectionStateChangedEventArgs(ConnectionState State, string? Message);