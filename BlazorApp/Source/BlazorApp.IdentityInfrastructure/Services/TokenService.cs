using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BlazorApp.Application.Identity.Exceptions;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Identity;
using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Shared.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using BlazorApp.Domain.Configurations;

namespace BlazorApp.CommonInfrastructure.Identity.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<BlazorAppUser> _userManager;
    private readonly JwtSettings _jwtSettings;

    public TokenService(
        UserManager<BlazorAppUser> userManager,
        IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<IResult<TokenResponse>> GetTokenAsync(TokenRequest request, string ipAddress)
    {
        var user = await _userManager.FindByEmailAsync(request.Email.Trim().Normalize());
        if (user == null)
        {
            throw new IdentityException("User Not Found.", statusCode: HttpStatusCode.Unauthorized);
        }

        if (!user.IsActive)
        {
            throw new IdentityException("User Not Active.Please contact the administrator.", statusCode: HttpStatusCode.Unauthorized);
        }

        bool passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!passwordValid)
        {
            throw new IdentityException("Provided Credentials are invalid.", statusCode: HttpStatusCode.Unauthorized);
        }

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        string token = GenerateJwt(user, ipAddress);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    public async Task<IResult<TokenResponse>> RefreshTokenAsync(RefreshTokenRequest request, string ipAddress)
    {
        if (request is null)
        {
            throw new IdentityException("Invalid Token.", statusCode: HttpStatusCode.Unauthorized);
        }

        var userPrincipal = GetPrincipalFromExpiredToken(request.Token);
        string userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            throw new IdentityException("User Not Found.", statusCode: HttpStatusCode.NotFound);
        }

        if (user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
        {
            throw new IdentityException("Invalid Refresh Token.", statusCode: HttpStatusCode.Unauthorized);
        }

        string token = GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays);
        await _userManager.UpdateAsync(user);
        var response = new TokenResponse(token, user.RefreshToken, user.RefreshTokenExpiryTime);
        return await Result<TokenResponse>.SuccessAsync(response);
    }

    private string GenerateJwt(BlazorAppUser user, string ipAddress)
    {
        return GenerateEncryptedToken(GetSigningCredentials(), GetClaims(user, ipAddress));
    }

    private IEnumerable<Claim> GetClaims(BlazorAppUser user, string ipAddress)
    {
        return new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id),
                new(ClaimTypes.Email, user.Email),
                new(ClaimConstants.Fullname, $"{user.FirstName} {user.LastName}"),
                new(ClaimTypes.Name, user.FirstName ?? string.Empty),
                new(ClaimTypes.Surname, user.LastName ?? string.Empty),
                new(ClaimConstants.IpAddress, ipAddress),
                new(ClaimConstants.ImageUrl, user.ImageUrl ?? string.Empty),
                new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty)
            };
    }

    private string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(
           claims: claims,
           expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes),
           signingCredentials: signingCredentials);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero,
            ValidateLifetime = false
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken ||
            !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
        {
            throw new IdentityException("Invalid Token.", statusCode: HttpStatusCode.Unauthorized);
        }

        return principal;
    }

    private SigningCredentials GetSigningCredentials()
    {
        if (string.IsNullOrEmpty(_jwtSettings.Key))
        {
            throw new InvalidOperationException("No Key defined in JwtSettings config.");
        }

        byte[] secret = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        return new SigningCredentials(new SymmetricSecurityKey(secret), SecurityAlgorithms.HmacSha256);
    }
}