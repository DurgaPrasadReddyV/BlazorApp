namespace BlazorApp.Shared.Identity;

public class ChangePasswordRequest
{
    public string? Password { get; set; }
    public string? NewPassword { get; set; }
    public string? ConfirmNewPassword { get; set; }
}
