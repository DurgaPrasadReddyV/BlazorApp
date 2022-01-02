using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Account.Events;

public class AccountUpdatedEvent : DomainEvent
{
    public AccountUpdatedEvent(Account account)
    {
        Account = account;
    }

    public Account Account { get; }
}