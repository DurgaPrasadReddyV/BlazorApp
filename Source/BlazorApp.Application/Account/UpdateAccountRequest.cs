using BlazorApp.Application.Common.Exceptions;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Domain.Dashboard;
using MediatR;
using Microsoft.Extensions.Localization;

namespace BlazorApp.Application.Account;

public class UpdateAccountRequest : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
}

public class UpdateAccountRequestHandler : IRequestHandler<UpdateAccountRequest, Guid>
{
    private readonly IRepository _repository;

    public UpdateAccountRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<Guid> Handle(UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await _repository.GetByIdAsync<BlazorApp.Domain.Account.Account>(request.Id, cancellationToken: cancellationToken);
        if (account is null)
        {
            throw new EntityNotFoundException(string.Format("{0} account not found", request.Id));
        }

        var updatedAccount = account.Update(request.Name);
        updatedAccount.DomainEvents.Add(new StatsChangedEvent());
        await _repository.UpdateAsync(updatedAccount, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}