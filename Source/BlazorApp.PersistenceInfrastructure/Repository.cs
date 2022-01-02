using System.Data;
using System.Linq.Expressions;
using BlazorApp.Application.Common.Exceptions;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Application.Wrapper;
using BlazorApp.CommonInfrastructure.Persistence;
using BlazorApp.CommonInfrastructure.Persistence.Contexts;
using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Shared;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace DN.WebApi.Infrastructure.Persistence;

public class Repository : IRepository
{
    private readonly ApplicationDbContext _dbContext;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<T>> GetListAsync<T>(
        Expression<Func<T, bool>>? condition = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
        where T : BaseEntity
    {
        return Filter(condition, orderBy, includes, asNoTracking: asNoTracking)
            .ToListAsync(cancellationToken);
    }


    public Task<List<T>> GetListAsync<T>(
        BaseSpecification<T>? specification = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(specification: specification, asNoTracking: asNoTracking)
            .ToListAsync(cancellationToken);
    }

    public Task<List<TProjectedType>> GetListAsync<T, TProjectedType>(
        Expression<Func<T, bool>>? condition = null,
        Expression<Func<T, TProjectedType>>? selectExpression = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>[]? includes = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        return Filter(condition, orderBy, includes, asNoTracking: true)
            .Select(selectExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<List<TProjectedType>> GetListAsync<T, TProjectedType>(
        Expression<Func<T, TProjectedType>> selectExpression,
        BaseSpecification<T>? specification = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        return Filter(specification: specification, asNoTracking: true)
            .Select(selectExpression)
            .ToListAsync(cancellationToken);
    }

    public Task<PaginatedResult<TDto>> GetListAsync<T, TDto>(
        PaginationSpecification<T> specification,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    where TDto : IDto
    {
        if (specification == null)
        {
            throw new ArgumentNullException(nameof(specification));
        }

        if (specification.PageIndex < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(specification.PageIndex),
                $"The value of {nameof(specification.PageIndex)} must be greater than or equal to 0.");
        }

        if (specification.PageSize < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(specification.PageSize),
                $"The value of {nameof(specification.PageSize)} must be greater than 0.");
        }

        IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

        if (specification.Filters is not null)
        {
            query = query.ApplyFilter(specification.Filters);
        }

        if (specification.AdvancedSearch?.Fields.Count > 0 &&
            !string.IsNullOrWhiteSpace(specification.AdvancedSearch.Keyword))
        {
            query = query.AdvancedSearch(specification.AdvancedSearch);
        }
        else if (!string.IsNullOrWhiteSpace(specification.Keyword))
        {
            query = query.SearchByKeyword(specification.Keyword);
        }

        query = query.Specify(specification);

        return query.ToMappedPaginatedResultAsync<T, TDto>(specification.PageIndex,
            specification.PageSize, cancellationToken);
    }

    // Get One By Id

    public Task<T?> GetByIdAsync<T>(
        Guid entityId,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(e => e.Id == entityId, includes: includes, asNoTracking: asNoTracking)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TProjectedType?> GetByIdAsync<T, TProjectedType>(
        Guid entityId,
        Expression<Func<T, TProjectedType>> selectExpression,
        Expression<Func<T, object>>[]? includes = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        return Filter(e => e.Id == entityId, includes: includes, asNoTracking: true)
            .Select(selectExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TDto> GetByIdAsync<T, TDto>(
        Guid entityId,
        Expression<Func<T, object>>[]? includes = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    where TDto : IDto
    {
        var entity = await GetByIdAsync(entityId, includes, true, cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(string.Format("{0} {1} not found.", typeof(T).Name, entityId));

        return entity.Adapt<TDto?>()!;
    }

    // Get One By Condition

    public Task<T?> GetAsync<T>(
        Expression<Func<T, bool>>? condition = null,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = false,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(condition, includes: includes, asNoTracking: asNoTracking)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public Task<T?> GetAsync<T>(
       BaseSpecification<T>? specification = null,
       bool asNoTracking = false,
       CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(specification: specification, asNoTracking: asNoTracking)
           .FirstOrDefaultAsync(cancellationToken);
    }


    public Task<TProjectedType?> GetAsync<T, TProjectedType>(
        Expression<Func<T, TProjectedType>> selectExpression,
        Expression<Func<T, bool>>? condition = null,
        Expression<Func<T, object>>[]? includes = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        return Filter(condition, includes: includes, asNoTracking: asNoTracking)
            .Select(selectExpression)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public Task<TProjectedType?> GetAsync<T, TProjectedType>(
        Expression<Func<T, TProjectedType>> selectExpression,
        BaseSpecification<T>? specification = null,
        bool asNoTracking = true,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (selectExpression == null)
            throw new ArgumentNullException(nameof(selectExpression));

        return Filter(specification: specification, asNoTracking: asNoTracking)
        .Select(selectExpression)
        .FirstOrDefaultAsync(cancellationToken);
    }

    // Get Count

    public Task<int> GetCountAsync<T>(
        Expression<Func<T, bool>>? condition = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(condition)
            .CountAsync(cancellationToken);
    }


    // Check if Exists

    public Task<bool> ExistsAsync<T>(
        Expression<Func<T, bool>> condition,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        return Filter(condition)
            .AnyAsync(cancellationToken);
    }


    // Filter

    private IQueryable<T> Filter<T>(
        Expression<Func<T, bool>>? condition = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        Expression<Func<T, object>>[]? includes = null,
        BaseSpecification<T>? specification = null,
        bool asNoTracking = false)
    where T : BaseEntity
    {
        IQueryable<T> query = _dbContext.Set<T>();

        if (condition is not null)
        {
            query = query.Where(condition);
        }

        if (specification is not null)
        {
            query = query.Specify(specification);
        }

        if (includes is not null)
        {
            query = query.IncludeMultiple(includes);
        }

        if (asNoTracking)
        {
            query = query.AsNoTracking();
        }

        if (orderBy is not null)
        {
            query = orderBy(query);
        }

        return query;
    }

    public async Task<Guid> CreateAsync<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        await _dbContext.Set<T>().AddAsync(entity, cancellationToken);
        return entity.Id;
    }

    public async Task<IList<Guid>> CreateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        await _dbContext.Set<T>().AddRangeAsync(entities, cancellationToken);
        return entities.Select(x => x.Id).ToList();
    }

    // Update

    public async Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        if (_dbContext.Entry(entity).State == EntityState.Unchanged)
        {
            throw new NothingToUpdateException();
        }

        var existing = await _dbContext.Set<T>().FindAsync(entity.Id);

        if (existing == null)
            throw new EntityNotFoundException(string.Format("{0} {1} not found.", typeof(T).Name, entity.Id));

        _dbContext.Entry(existing).CurrentValues.SetValues(entity);
    }

    public async Task UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        foreach (var entity in entities)
        {
            if (_dbContext.Entry(entity).State == EntityState.Unchanged)
            {
                throw new NothingToUpdateException();
            }

            var existing = await _dbContext.Set<T>().FindAsync(entity.Id);

            if (existing == null)
                throw new EntityNotFoundException(string.Format("{0} {1} not found.", typeof(T).Name, entity.Id));

            _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }
    }

    // Delete

    public void Remove<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        _dbContext.Set<T>().Remove(entity);
    }

    public async Task<T> RemoveByIdAsync<T>(Guid entityId, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        var entity = await _dbContext.Set<T>().FindAsync(new object?[] { entityId }, cancellationToken: cancellationToken);

        if (entity == null)
            throw new EntityNotFoundException(string.Format("{0} {1} not found.", typeof(T).Name, entityId));

        _dbContext.Set<T>().Remove(entity);

        return entity;
    }

    public void RemoveRange<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        foreach (var entity in entities)
        {
            _dbContext.Set<T>().Remove(entity);
        }
    }

    public void Clear<T>(
        Expression<Func<T, bool>>? condition = null,
        BaseSpecification<T>? specification = null,
        CancellationToken cancellationToken = default)
    where T : BaseEntity
    {
        Filter(condition, specification: specification)
            .ForEachAsync(
                x =>
            {
                _dbContext.Entry(x).State = EntityState.Deleted;
            }, cancellationToken: cancellationToken);
    }

    // SaveChanges

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}