using BlazorApp.Client.ApiClient;

namespace BlazorApp.Client.Authentication;

public interface IAuthenticationService
{
    Task<Result> LoginAsync(TokenRequest request);

    Task LogoutAsync();
}