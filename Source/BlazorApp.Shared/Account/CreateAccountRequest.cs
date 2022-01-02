

namespace BlazorApp.Shared.Account;

public class CreateAccountRequest 
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Rate { get; set; }
    public Guid BrandId { get; set; }
}