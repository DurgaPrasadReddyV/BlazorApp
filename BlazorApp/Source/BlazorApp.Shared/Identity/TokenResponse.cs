namespace BlazorApp.Shared.Identity;

public record TokenResponse(string Token, string RefreshToken, DateTime RefreshTokenExpiryTime);