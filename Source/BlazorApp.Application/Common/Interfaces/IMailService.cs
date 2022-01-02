using BlazorApp.Shared.Mailing;

namespace BlazorApp.Application.Common.Interfaces;

public interface IMailService 
{
    Task SendAsync(MailRequest request);
}