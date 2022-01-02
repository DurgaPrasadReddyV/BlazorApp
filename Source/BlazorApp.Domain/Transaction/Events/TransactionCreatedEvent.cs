using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Transaction.Events;

public class TransactionCreatedEvent : DomainEvent
{
    public TransactionCreatedEvent(Transaction transaction)
    {
        Transaction = transaction;
    }

    public Transaction Transaction { get; }
}