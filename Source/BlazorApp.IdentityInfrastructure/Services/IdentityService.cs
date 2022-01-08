using System.Net;
using System.Security.Claims;
using System.Text;
using BlazorApp.Application.FileStorage;
using BlazorApp.Application.Identity.Exceptions;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Common;
using BlazorApp.Domain.Identity;
using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Shared.Identity;
using BlazorApp.Shared.Mailing;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using BlazorApp.Application.Common.Interfaces;

namespace BlazorApp.CommonInfrastructure.Identity.Services;

public class IdentityService : IIdentityService
{
    private readonly SignInManager<BlazorAppIdentityUser> _signInManager;
    private readonly UserManager<BlazorAppIdentityUser> _userManager;
    private readonly RoleManager<BlazorAppIdentityRole> _roleManager;
    private readonly IFileStorageService _fileStorage;
    private readonly IEmailTemplateService _templateService;
    private readonly IMailService _mailService;
    private readonly IJobService _jobService;

    private readonly IRepository _repository;

    public IdentityService(
        SignInManager<BlazorAppIdentityUser> signInManager,
        UserManager<BlazorAppIdentityUser> userManager,
        RoleManager<BlazorAppIdentityRole> roleManager,
        IFileStorageService fileStorage,
        IEmailTemplateService templateService,
        IMailService mailService,
        IJobService jobService,
        IRepository repository)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _roleManager = roleManager;
        _fileStorage = fileStorage;
        _templateService = templateService;
        _mailService = mailService;
        _jobService = jobService;
        _repository = repository;
    }

    public async Task<IResult<string>> RegisterAsync(RegisterUserRequest request, string origin)
    {
        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            throw new IdentityException(string.Format("Username {0} is already taken.", request.UserName));
        }

        var user = new BlazorAppIdentityUser
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true
        };

        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber);
            if (userWithSamePhoneNumber != null)
            {
                throw new IdentityException(string.Format("Phone number {0} is already registered.", request.PhoneNumber));
            }
        }

        if (await _userManager.FindByEmailAsync(request.Email?.Normalize()) is not null)
        {
            throw new IdentityException(string.Format("Email {0} is already registered.", request.Email));
        }

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            throw new IdentityException("Validation Errors Occurred.", result.Errors.Select(a => a.Description.ToString()).ToList());
        }

        var createdUser  = await _userManager.FindByEmailAsync(request.Email?.Normalize());
        var blazorAppUser = new BlazorAppUser(Guid.Parse(createdUser.Id));
        await _repository.CreateAsync<BlazorAppUser>(blazorAppUser);
        await _repository.SaveChangesAsync();

        await _userManager.AddToRoleAsync(user, DefaultRoles.Admin);

        var messages = new List<string> { string.Format("User {0} Registered.", user.UserName) };

        // send verification email
        string emailVerificationUri = await GetEmailVerificationUriAsync(user, origin);
        var mailRequest = new MailRequest( new List<string> { user.Email! },"Confirm Registration",
            _templateService.GenerateEmailConfirmationMail(user.UserName ?? "User", user.Email!, emailVerificationUri));
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        
        messages.Add($"Please check {user.Email} to verify your account!");

        return await Result<string>.SuccessAsync(user.Id, messages: messages);
    }

    public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
    {
        var user = await _userManager.Users.IgnoreQueryFilters().Where(a => a.Id == userId && !a.EmailConfirmed).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new IdentityException("An error occurred while confirming E-Mail.");
        }

        code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
        var result = await _userManager.ConfirmEmailAsync(user, code);
        if (result.Succeeded)
        {
            return await Result<string>.SuccessAsync(user.Id, string.Format("Account Confirmed for E-Mail {0}. You can now use the /api/identity/token endpoint to generate JWT.", user.Email));
        }
        else
        {
            throw new IdentityException(string.Format("An error occurred while confirming {0}", user.Email));
        }
    }

    public async Task<IResult<string>> ConfirmPhoneNumberAsync(string userId, string code)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new IdentityException("An error occurred while confirming Mobile Phone.");
        }

        var result = await _userManager.ChangePhoneNumberAsync(user, user.PhoneNumber, code);
        if (result.Succeeded)
        {
            if (user.EmailConfirmed)
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format("Account Confirmed for Phone Number {0}. You can now use the /api/identity/token endpoint to generate JWT.", user.PhoneNumber));
            }
            else
            {
                return await Result<string>.SuccessAsync(user.Id, string.Format("Account Confirmed for Phone Number {0}. You should confirm your E-mail before using the /api/identity/token endpoint to generate JWT.", user.PhoneNumber));
            }
        }
        else
        {
            throw new IdentityException(string.Format("An error occurred while confirming {0}", user.PhoneNumber));
        }
    }

    public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
    {
        var user = await _userManager.FindByEmailAsync(request?.Email?.Normalize());
        if (user is null || !await _userManager.IsEmailConfirmedAsync(user))
        {
            // Don't reveal that the user does not exist or is not confirmed
            throw new IdentityException("An Error has occurred!");
        }

        string code = await _userManager.GeneratePasswordResetTokenAsync(user);
        const string route = "account/reset-password";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code);
        var mailRequest = new MailRequest(
            new List<string> { request?.Email! },
            "Reset Password",
            $"Your Password Reset Token is '{code}'. You can reset your password using the {endpointUri} Endpoint.");
        _jobService.Enqueue(() => _mailService.SendAsync(mailRequest));
        return await Result.SuccessAsync("Password Reset Mail has been sent to your authorized Email.");
    }

    public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email?.Normalize());
        if (user == null)
        {
            // Don't reveal that the user does not exist
            throw new IdentityException("An Error has occurred!");
        }

        var result = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
        if (result.Succeeded)
        {
            return await Result.SuccessAsync("Password Reset Successful!");
        }
        else
        {
            throw new IdentityException("An Error has occurred!");
        }
    }

    public async Task<IResult> UpdateProfileAsync(UpdateProfileRequest request, string userId)
    {
        if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
        {
            var userWithSamePhoneNumber = await _userManager.Users.FirstOrDefaultAsync(x => x.PhoneNumber == request.PhoneNumber && x.Id != userId);
            if (userWithSamePhoneNumber != null)
            {
                return await Result.FailAsync(string.Format("Phone number {0} is already used.", request.PhoneNumber));
            }
        }

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email?.Normalize());
        if (userWithSameEmail == null || userWithSameEmail.Id == userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await Result.FailAsync("User Not Found.");
            }

            string currentImage = user.ImageUrl ?? string.Empty;

            if (request.Image != null)
            {
                user.ImageUrl = await _fileStorage.UploadAsync<BlazorAppIdentityUser>(request.Image, FileType.Image);
                if (!string.IsNullOrEmpty(currentImage))
                {
                    string root = Directory.GetCurrentDirectory();
                    string filePath = currentImage.Replace("{server_url}/", string.Empty);
                    _fileStorage.Remove(Path.Combine(root, filePath));
                }
            }

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.PhoneNumber = request.PhoneNumber;
            string phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (request.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, request.PhoneNumber);
            }

            var identityResult = await _userManager.UpdateAsync(user);
            var errors = identityResult.Errors.Select(e => e.Description.ToString()).ToList();
            await _signInManager.RefreshSignInAsync(user);
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }
        else
        {
            return await Result.FailAsync(string.Format("Email {0} is already used.", request.Email));
        }
    }

    public async Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return await Result.FailAsync("User Not Found.");
        }

        var identityResult = await _userManager.ChangePasswordAsync(
            user,
            model.Password,
            model.NewPassword);
        var errors = identityResult.Errors.Select(e => e.Description.ToString()).ToList();
        return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
    }

    private async Task<string> GetMobilePhoneVerificationCodeAsync(BlazorAppIdentityUser user)
    {
        return await _userManager.GenerateChangePhoneNumberTokenAsync(user, user.PhoneNumber);
    }

    private async Task<string> GetEmailVerificationUriAsync(BlazorAppIdentityUser user, string origin)
    {
        string code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
        const string route = "api/identity/confirm-email/";
        var endpointUri = new Uri(string.Concat($"{origin}/", route));
        string verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), QueryConstants.UserId, user.Id);
        verificationUri = QueryHelpers.AddQueryString(verificationUri, QueryConstants.Code, code);
        return verificationUri;
    }
}