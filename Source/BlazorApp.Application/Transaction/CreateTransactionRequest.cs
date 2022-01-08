using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Domain.Dashboard;
using MediatR;

namespace BlazorApp.Application.Transaction;

public class CreateTransactionRequest : IRequest<Guid>
{
    public string Name { get; set; } = default!;
}

public class CreateTransactionRequestHandler : IRequestHandler<CreateTransactionRequest, Guid>
{
    private readonly IRepository _repository;
    
    public CreateTransactionRequestHandler(IRepository repository) 
    { 
        _repository = repository;
    }

    public async Task<Guid> Handle(CreateTransactionRequest request, CancellationToken cancellationToken)
    {
        var transaction = new BlazorApp.Domain.Transaction.Transaction(request.Name);

        transaction.DomainEvents.Add(new StatsChangedEvent());

        await _repository.CreateAsync(transaction, cancellationToken);

        await _repository.SaveChangesAsync(cancellationToken);

        return transaction.Id;
    }
}