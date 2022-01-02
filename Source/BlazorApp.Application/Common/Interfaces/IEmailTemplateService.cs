namespace BlazorApp.Application.Common.Interfaces;

public interface IEmailTemplateService
{
    string GenerateEmailConfirmationMail(string userName, string email, string emailVerificationUri);
}