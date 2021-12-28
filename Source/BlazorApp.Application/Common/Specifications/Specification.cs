using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Shared.Filters;

namespace BlazorApp.Application.Common.Specifications;

public class Specification<T> : BaseSpecification<T>
where T : BaseEntity
{
    public string? Keyword { get; set; }
    public Search? AdvancedSearch { get; set; }
    public Filters<T>? Filters { get; set; }
}
