using BlazorApp.Shared;

namespace BlazorApp.Application.Transaction;

public class TransactionDto : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
}