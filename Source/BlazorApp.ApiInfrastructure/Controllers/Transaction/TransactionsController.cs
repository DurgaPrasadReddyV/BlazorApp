using BlazorApp.CommonInfrastructure.Identity.Permissions;
using BlazorApp.Host.Controllers;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Domain.Identity;
using NSwag.Annotations;
using BlazorApp.Application.Wrapper;
using BlazorApp.Application.Transaction;

public class TransactionsController : BaseController
{
    [HttpPost("search")]
    [MustHavePermission(Permissions.Transactions.Search)]
    [OpenApiOperation("Search transactions using available Filters.", "")]
    public Task<PaginatedResult<TransactionDto>> SearchAsync(SearchTransactionsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [MustHavePermission(Permissions.Transactions.Register)]
    public Task<Guid> CreateAsync(CreateTransactionRequest request)
    {
        return Mediator.Send(request);
    }
}