using BlazorApp.Shared.Filters;

namespace BlazorApp.Shared.Account;

public class AccountListFilter : PaginationFilter
{
    public Guid? BrandId { get; set; }

    public decimal? MinimumRate { get; set; }

    public decimal? MaximumRate { get; set; }
}