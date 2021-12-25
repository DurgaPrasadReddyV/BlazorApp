using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Infrastructure.Persistence;

public static class QueryableExtensions
{
    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> specification)
    where T : BaseEntity
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        if (specification.Conditions?.Any() == true)
        {
            foreach (var specificationCondition in specification.Conditions)
            {
                query = query.Where(specificationCondition);
            }
        }

        if (specification.Includes != null)
        {
            query = query.IncludeMultiple(specification.Includes);
        }

        if (specification.OrderByStrings?.Length > 0)
        {
            return query.ApplySort(specification.OrderByStrings);
        }
        else if (specification.OrderBy != null)
        {
            return specification.OrderBy(query);
        }

        return query;
    }

    public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> query, Filters<T>? filters)
    {
        if (filters?.IsValid() == true)
            query = filters.Get().Aggregate(query, (current, filter) => current.Where(filter.Expression));
        return query;
    }

    public static IQueryable<T> ApplySort<T>(this IQueryable<T> query, string[]? orderBy)
    where T : BaseEntity
    {
        string? ordering = new OrderByConverter().ConvertBack(orderBy);
        return !string.IsNullOrWhiteSpace(ordering) ? query.OrderBy(ordering) : query.OrderBy(a => a.Id);
    }

    public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
    where T : BaseEntity
    {
        if (includes != null)
        {
            query = includes.Aggregate(query, (current, include) => current.Include(include.AsPath()));
        }

        return query;
    }
}