using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Validation;
using FluentValidation;

namespace BlazorApp.Application.Transaction;

public class CreateTransactionRequestValidator : CustomValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator(IRepository repository)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => !await repository.ExistsAsync<BlazorApp.Domain.Transaction.Transaction>(
                a => a.Name == name, ct))
                .WithMessage((_, name) => string.Format("{0} Transaction already exists", name));
    }
}