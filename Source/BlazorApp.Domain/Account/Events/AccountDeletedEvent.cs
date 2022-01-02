using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Account.Events;

public class AccountDeletedEvent : DomainEvent
{
    public AccountDeletedEvent(Account account)
    {
        Account = account;
    }

    public Account Account { get; }
}