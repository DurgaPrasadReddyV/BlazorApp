using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Application.Common.Specifications;

public class PaginationSpecification<T> : Specification<T>
where T : BaseEntity
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = int.MaxValue;
}