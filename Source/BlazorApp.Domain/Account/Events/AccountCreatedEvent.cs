using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Account.Events;

public class AccountCreatedEvent : DomainEvent
{
    public AccountCreatedEvent(Account account)
    {
        Account = account;
    }

    public Account Account { get; }
}