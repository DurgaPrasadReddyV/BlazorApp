using BlazorApp.Client.Infrastructure.ApiClient;

namespace BlazorApp.Client.Infrastructure.Authentication;

public interface IAuthenticationService
{
    Task<Result> LoginAsync(TokenRequest request);

    Task LogoutAsync();
}