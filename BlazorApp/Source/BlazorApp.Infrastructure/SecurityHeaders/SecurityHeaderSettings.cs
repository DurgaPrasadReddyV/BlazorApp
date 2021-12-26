namespace BlazorApp.Infrastructure.SecurityHeaders;

public class SecurityHeaderSettings
{
    public bool Enable { get; set; }

    public string? XFrameOptions { get; set; }

    public string? XContentTypeOptions { get; set; }

    public string? ReferrerPolicy { get; set; }

    public string? PermissionsPolicy { get; set; }

    public string? SameSite { get; set; }

    public string? XXSSProtection { get; set; }
}