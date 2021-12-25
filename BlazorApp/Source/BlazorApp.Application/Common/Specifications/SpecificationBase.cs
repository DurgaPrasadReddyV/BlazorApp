using System.Linq.Expressions;
using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Application.Common.Specifications;

public class BaseSpecification<T> : ISpecification<T>
where T : BaseEntity
{
    public List<Expression<Func<T, bool>>> Conditions { get; set; } = new List<Expression<Func<T, bool>>>();

    public Expression<Func<T, object>>[] Includes { get; set; } = default!;

    public Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; } = default!;

    public string[]? OrderByStrings { get; set; }
}