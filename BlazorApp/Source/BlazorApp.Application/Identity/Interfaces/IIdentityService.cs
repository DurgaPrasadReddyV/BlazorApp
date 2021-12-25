using System.Security.Claims;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Identity;

namespace BlazorApp.Application.Identity.Interfaces;

public interface IIdentityService : ITransientService
{
    Task<string> GetOrCreateFromPrincipalAsync(ClaimsPrincipal principal);

    Task<IResult<string>> RegisterAsync(RegisterRequest request, string origin);

    Task<IResult<string>> ConfirmEmailAsync(string userId, string code, string tenant);

    Task<IResult<string>> ConfirmPhoneNumberAsync(string userId, string code);

    Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

    Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

    Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId);

    Task<IResult> ChangePasswordAsync(ChangePasswordRequest request, string userId);
}