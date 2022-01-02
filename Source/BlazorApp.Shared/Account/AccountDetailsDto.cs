namespace BlazorApp.Shared.Account;

public class AccountDetailsDto : IDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public decimal Rate { get; set; }
}