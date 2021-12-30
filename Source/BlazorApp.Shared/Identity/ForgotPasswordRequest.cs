using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared.Identity;

public class ForgotPasswordRequest
{
    public string Email { get; set; } = default!;
}