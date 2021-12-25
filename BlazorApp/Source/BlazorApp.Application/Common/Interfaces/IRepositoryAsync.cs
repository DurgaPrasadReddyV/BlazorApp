using System.Data;
using System.Linq.Expressions;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Shared;

namespace BlazorApp.Application.Common.Interfaces;

public interface IRepositoryAsync : ITransientService
{
    Task<List<T>> GetListAsync<T>(Expression<Func<T, bool>>? condition = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Expression<Func<T, object>>[]? includes = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<List<T>> GetListAsync<T>(BaseSpecification<T>? specification = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<List<TProjectedType>> GetListAsync<T, TProjectedType>(Expression<Func<T, bool>>? condition = null, Expression<Func<T, TProjectedType>>? selectExpression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Expression<Func<T, object>>[]? includes = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<List<TProjectedType>> GetListAsync<T, TProjectedType>(Expression<Func<T, TProjectedType>> selectExpression, BaseSpecification<T>? specification = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<PaginatedResult<TDto>> GetListAsync<T, TDto>(PaginationSpecification<T> specification, CancellationToken cancellationToken = default)
    where T : BaseEntity
    where TDto : IDto;

    Task<T?> GetByIdAsync<T>(Guid entityId, Expression<Func<T, object>>[]? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<TProjectedType?> GetByIdAsync<T, TProjectedType>(Guid entityId, Expression<Func<T, TProjectedType>> selectExpression, Expression<Func<T, object>>[]? includes = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<TDto> GetByIdAsync<T, TDto>(Guid entityId, Expression<Func<T, object>>[]? includes = null, CancellationToken cancellationToken = default)
    where T : BaseEntity
    where TDto : IDto;
    
    Task<T?> GetAsync<T>(Expression<Func<T, bool>>? condition = null, Expression<Func<T, object>>[]? includes = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<T?> GetAsync<T>(BaseSpecification<T>? specification = null, bool asNoTracking = false, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<TProjectedType?> GetAsync<T, TProjectedType>(Expression<Func<T, TProjectedType>> selectExpression, Expression<Func<T, bool>>? condition = null, Expression<Func<T, object>>[]? includes = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<TProjectedType?> GetAsync<T, TProjectedType>(Expression<Func<T, TProjectedType>> selectExpression, BaseSpecification<T>? specification = null, bool asNoTracking = true, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<int> GetCountAsync<T>(Expression<Func<T, bool>>? condition = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<bool> ExistsAsync<T>(Expression<Func<T, bool>> condition, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<Guid> CreateAsync<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<IList<Guid>> CreateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task UpdateRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task RemoveAsync<T>(T entity, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task RemoveRangeAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<T> RemoveByIdAsync<T>(Guid entityId, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task ClearAsync<T>(Expression<Func<T, bool>>? expression = null, BaseSpecification<T>? specification = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;

    Task<T> QuerySingleAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    where T : BaseEntity;
}