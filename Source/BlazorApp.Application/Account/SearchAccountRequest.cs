using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Filters;
using MediatR;

namespace BlazorApp.Application.Account;

public class SearchAccountsRequest : PaginationFilter, IRequest<PaginatedResult<AccountDto>>
{
}

public class SearchAccountsRequestHandler : IRequestHandler<SearchAccountsRequest, PaginatedResult<AccountDto>>
{
    private readonly IRepository _repository;

    public SearchAccountsRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<PaginatedResult<AccountDto>> Handle(SearchAccountsRequest request, CancellationToken cancellationToken)
    {
        var specification = new PaginationSpecification<BlazorApp.Domain.Account.Account>
        {
            AdvancedSearch = request.AdvancedSearch,
            Keyword = request.Keyword,
            OrderBy = x => x.OrderBy(b => b.Name),
            OrderByStrings = request.OrderBy,
            PageIndex = request.PageNumber,
            PageSize = request.PageSize,
        };

        return await _repository.GetListAsync<BlazorApp.Domain.Account.Account, AccountDto>(specification, cancellationToken);
    }
}