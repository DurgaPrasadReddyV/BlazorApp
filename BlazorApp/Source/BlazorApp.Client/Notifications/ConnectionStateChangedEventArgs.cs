namespace BlazorApp.Client.Notifications;

public record ConnectionStateChangedEventArgs(ConnectionState State, string? Message);