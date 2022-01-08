using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Domain.Dashboard;
using MediatR;

namespace BlazorApp.Application.Account;

public class DeleteAccountRequest : IRequest<Guid>
{
    public Guid Id { get; set; }

    public DeleteAccountRequest(Guid id) => Id = id;
}

public class DeleteAccountRequestHandler : IRequestHandler<DeleteAccountRequest, Guid>
{
    private readonly IRepository _repository;

    public DeleteAccountRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<Guid> Handle(DeleteAccountRequest request, CancellationToken cancellationToken)
    {
        var accountToDelete = await _repository.RemoveByIdAsync<BlazorApp.Domain.Account.Account>(request.Id, cancellationToken);
        accountToDelete.DomainEvents.Add(new StatsChangedEvent());

        await _repository.SaveChangesAsync(cancellationToken);

        return request.Id;
    }
}