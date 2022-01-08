using BlazorApp.Shared;

namespace BlazorApp.Application.Account;

public class AccountDto : IDto
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public Guid BlazorAppUserId {get; set;}
}