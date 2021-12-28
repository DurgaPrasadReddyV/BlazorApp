using BlazorApp.Shared.Filters;

namespace BlazorApp.Shared.Identity;

public class UserListFilter : PaginationFilter
{
    public bool? IsActive { get; set; }
}