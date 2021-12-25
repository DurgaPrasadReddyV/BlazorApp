namespace BlazorApp.Domain.Common;

public static class StringExtensions
{
    public static string NullToString(this object? Value)
        => Value?.ToString() ?? string.Empty;
}