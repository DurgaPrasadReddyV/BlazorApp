using BlazorApp.CommonInfrastructure.Identity.Permissions;
using BlazorApp.Host.Controllers;
using Microsoft.AspNetCore.Mvc;
using BlazorApp.Domain.Identity;
using NSwag.Annotations;
using BlazorApp.Application.Wrapper;
using BlazorApp.Application.Account;

public class AccountsController : BaseController
{
    [HttpPost("search")]
    [MustHavePermission(Permissions.Accounts.Search)]
    [OpenApiOperation("Search accounts using available Filters.", "")]
    public Task<PaginatedResult<AccountDto>> SearchAsync(SearchAccountsRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPost]
    [MustHavePermission(Permissions.Accounts.Register)]
    public Task<Guid> CreateAsync(CreateAccountRequest request)
    {
        return Mediator.Send(request);
    }

    [HttpPut("{id:guid}")]
    [MustHavePermission(Permissions.Accounts.Update)]
    public async Task<ActionResult<Guid>> UpdateAsync(UpdateAccountRequest request, Guid id)
    {
        if (id != request.Id)
        {
            return BadRequest();
        }

        return Ok(await Mediator.Send(request));
    }

    [HttpDelete("{id:guid}")]
    [MustHavePermission(Permissions.Accounts.Remove)]
    public Task<Guid> DeleteAsync(Guid id)
    {
        return Mediator.Send(new DeleteAccountRequest(id));
    }
}