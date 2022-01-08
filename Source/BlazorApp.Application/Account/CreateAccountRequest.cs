using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Domain.Dashboard;
using MediatR;

namespace BlazorApp.Application.Account;

public class CreateAccountRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
    public Guid BlazorAppUserId {get; set;}
}

public class CreateAccountRequestHandler : IRequestHandler<CreateAccountRequest, Guid>
{
    private readonly IRepository _repository;
    
    public CreateAccountRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = new BlazorApp.Domain.Account.Account(request.Name, request.BlazorAppUserId);

        account.DomainEvents.Add(new StatsChangedEvent());

        await _repository.CreateAsync(account, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return account.Id;
    }
}