using System.ComponentModel.DataAnnotations;

namespace BlazorApp.Shared.Identity;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    public string? Email { get; set; }
}