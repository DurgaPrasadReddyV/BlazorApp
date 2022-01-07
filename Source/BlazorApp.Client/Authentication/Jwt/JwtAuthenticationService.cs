using BlazorApp.Client.ApiClient;
using BlazorApp.Client.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace BlazorApp.Client.Authentication.Jwt;

public class JwtAuthenticationService : AuthenticationStateProvider, IAuthenticationService, IAccessTokenProvider
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private readonly ILocalStorageService _localStorage;
    private readonly ITokensClient _tokensClient;
    private readonly IUsersClient _usersClient;
    private readonly NavigationManager _navigation;

    public JwtAuthenticationService(ILocalStorageService localStorage, IUsersClient usersClient, ITokensClient tokensClient, NavigationManager navigation)
    {
        _localStorage = localStorage;
        _usersClient = usersClient;
        _tokensClient = tokensClient;
        _navigation = navigation;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        string cachedToken = await GetCachedAuthTokenAsync();
        if (string.IsNullOrWhiteSpace(cachedToken))
        {
            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // Generate claimsIdentity from cached token
        var claimsIdentity = new ClaimsIdentity(GetClaimsFromJwt(cachedToken), "jwt");

        // Add cached permissions as claims
        if (await GetCachedPermissionsAsync() is List<string> cachedPermissions)
        {
            claimsIdentity.AddClaims(cachedPermissions.Select(p => new Claim(FSHClaims.Permission, p)));
        }

        return new AuthenticationState(new ClaimsPrincipal(claimsIdentity));
    }

    public async Task<Result> LoginAsync(TokenRequest request)
    {
        var result = await _tokensClient.GetTokenAsync(request);
        if (result.Succeeded)
        {
            string? token = result.Data?.Token;
            string? refreshToken = result.Data?.RefreshToken;

            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(refreshToken))
            {
                return new Result { Succeeded = false, Messages = new List<string>() { "Invalid token received." } };
            }

            await CacheAuthTokens(token, refreshToken);

            // Get permissions for this user and add them to the cache
            var claims = GetClaimsFromJwt(token);
            string? userId = claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var permissionResult = await _usersClient.GetPermissionsAsync(userId);
                if (permissionResult.Succeeded && permissionResult.Data is not null)
                {
                    await CachePermissions(permissionResult.Data
                        .Where(p => !string.IsNullOrWhiteSpace(p?.Permission))
                        .Select(p => p!.Permission!)
                        .ToList());
                }
            }

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        else
        {
            await ClearCacheAsync();
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
        return result;
    }

    public async Task LogoutAsync()
    {
        await ClearCacheAsync();

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());

        _navigation.NavigateTo("/login");
    }

    public async Task ReLoginAsync(string returnUrl)
    {
        await LogoutAsync();
        _navigation.NavigateTo(returnUrl);
    }

    public async ValueTask<AccessTokenResult> RequestAccessToken()
    {
        var authState = await GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated is not true)
        {
            return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
        }

        // We make sure the access token is only refreshed by one thread at a time. The other ones have to wait.
        await _semaphore.WaitAsync();
        try
        {
            string? token = await GetCachedAuthTokenAsync();

            // Check if token needs to be refreshed (when its expiration time is less than 1 minute away)
            var expTime = authState.User.GetExpiration();
            var diff = expTime - DateTime.UtcNow;
            if (diff.TotalMinutes <= 1)
            {
                string? refreshToken = await GetCachedRefreshTokenAsync();
                var response = await RefreshTokenAsync(new RefreshTokenRequest { Token = token, RefreshToken = refreshToken });
                if (!response.Succeeded)
                {
                    return new AccessTokenResult(AccessTokenResultStatus.RequiresRedirect, null, "/login");
                }

                token = response.Data?.Token;
            }

            return new AccessTokenResult(AccessTokenResultStatus.Success, new AccessToken() { Value = token }, string.Empty);
        }
        catch
        {
            await ClearCacheAsync();
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public ValueTask<AccessTokenResult> RequestAccessToken(AccessTokenRequestOptions options) =>
        RequestAccessToken();

    private async Task<ResultOfTokenResponse> RefreshTokenAsync(RefreshTokenRequest request)
    {
        var authState = await GetAuthenticationStateAsync();
        try
        {
            var tokenResponse = await _tokensClient.RefreshAsync(request);
            if (tokenResponse.Succeeded && tokenResponse.Data is not null)
            {
                await CacheAuthTokens(tokenResponse.Data.Token, tokenResponse.Data.RefreshToken);
            }

            return tokenResponse;
        }
        catch (ApiException<ErrorResult>)
        {
            return new ResultOfTokenResponse { Succeeded = false };
        }
    }

    private async ValueTask CacheAuthTokens(string? token, string? refreshToken)
    {
        await _localStorage.SetItemAsync(StorageConstants.Local.AuthToken, token);
        await _localStorage.SetItemAsync(StorageConstants.Local.RefreshToken, refreshToken);
    }

    private async ValueTask CachePermissions(List<string> permissions) =>
        await _localStorage.SetItemAsync(StorageConstants.Local.Permissions, permissions);

    private async Task ClearCacheAsync()
    {
        await _localStorage.RemoveItemAsync(StorageConstants.Local.AuthToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.RefreshToken);
        await _localStorage.RemoveItemAsync(StorageConstants.Local.Permissions);
    }

    private async ValueTask<string> GetCachedAuthTokenAsync() =>
        await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);

    private async ValueTask<string> GetCachedRefreshTokenAsync() =>
        await _localStorage.GetItemAsync<string>(StorageConstants.Local.RefreshToken);

    private async ValueTask<List<string>> GetCachedPermissionsAsync() =>
        await _localStorage.GetItemAsync<List<string>>(StorageConstants.Local.Permissions);

    private IEnumerable<Claim> GetClaimsFromJwt(string jwt)
    {
        var claims = new List<Claim>();
        string payload = jwt.Split('.')[1];
        byte[] jsonBytes = ParseBase64WithoutPadding(payload);
        var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

        if (keyValuePairs is not null)
        {
            keyValuePairs.TryGetValue(ClaimTypes.Role, out object? roles);

            if (roles is not null)
            {
                string? rolesString = roles.ToString();
                if (!string.IsNullOrEmpty(rolesString))
                {
                    if (rolesString.Trim().StartsWith("["))
                    {
                        string[]? parsedRoles = JsonSerializer.Deserialize<string[]>(rolesString);

                        if (parsedRoles is not null)
                        {
                            claims.AddRange(parsedRoles.Select(role => new Claim(ClaimTypes.Role, role)));
                        }
                    }
                    else
                    {
                        claims.Add(new Claim(ClaimTypes.Role, rolesString));
                    }
                }

                keyValuePairs.Remove(ClaimTypes.Role);
            }

            claims.AddRange(keyValuePairs.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString() ?? string.Empty)));
        }

        return claims;
    }

    private byte[] ParseBase64WithoutPadding(string payload)
    {
        payload = payload.Trim().Replace('-', '+').Replace('_', '/');
        string base64 = payload.PadRight(payload.Length + ((4 - (payload.Length % 4)) % 4), '=');
        return Convert.FromBase64String(base64);
    }
}