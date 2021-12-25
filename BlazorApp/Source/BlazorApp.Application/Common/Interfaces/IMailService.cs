using BlazorApp.Shared.Mailing;

namespace BlazorApp.Application.Common.Interfaces;

public interface IMailService : ITransientService
{
    Task SendAsync(MailRequest request);
}