using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Filters;
using MediatR;

namespace BlazorApp.Application.Transaction;

public class SearchTransactionsRequest : PaginationFilter, IRequest<PaginatedResult<TransactionDto>>
{
}

public class SearchTransactionsRequestHandler : IRequestHandler<SearchTransactionsRequest, PaginatedResult<TransactionDto>>
{
    private readonly IRepository _repository;

    public SearchTransactionsRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<PaginatedResult<TransactionDto>> Handle(SearchTransactionsRequest request, CancellationToken cancellationToken)
    {
        var specification = new PaginationSpecification<BlazorApp.Domain.Transaction.Transaction>
        {
            AdvancedSearch = request.AdvancedSearch,
            Keyword = request.Keyword,
            OrderBy = x => x.OrderBy(b => b.Name),
            OrderByStrings = request.OrderBy,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
        };

        return await _repository.GetListAsync<BlazorApp.Domain.Transaction.Transaction, TransactionDto>(specification, cancellationToken);
    }
}