using System.Linq.Expressions;
using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Application.Common.Specifications;

public interface ISpecification<T>
where T : BaseEntity
{
    List<Expression<Func<T, bool>>> Conditions { get; set; }

    Expression<Func<T, object>>[] Includes { get; set; }

    Func<IQueryable<T>, IOrderedQueryable<T>> OrderBy { get; set; }

    string[]? OrderByStrings { get; set; }
}