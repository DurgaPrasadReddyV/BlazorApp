using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Identity;

namespace BlazorApp.Application.Identity.Interfaces;

public interface ITokenService : ITransientService
{
    Task<IResult<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress);

    Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress);
}