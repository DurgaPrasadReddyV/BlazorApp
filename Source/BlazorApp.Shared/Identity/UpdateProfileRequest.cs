using BlazorApp.Shared.FileStorage;

namespace BlazorApp.Shared.Identity;

public class UpdateProfileRequest
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public FileUploadRequest? Image { get; set; }
}