namespace BlazorApp.Domain.Configurations;

public class SecurityHeaderSettings
{
    public string? XFrameOptions { get; set; }

    public string? XContentTypeOptions { get; set; }

    public string? XXSSProtection { get; set; }

    public string? ReferrerPolicy { get; set; }

    public string? PermissionsPolicy { get; set; }

    public string? SameSite { get; set; }
}