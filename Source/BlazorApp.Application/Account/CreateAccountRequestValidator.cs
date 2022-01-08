using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Validation;
using FluentValidation;

namespace BlazorApp.Application.Account;

public class CreateAccountRequestValidator : CustomValidator<CreateAccountRequest>
{
    public CreateAccountRequestValidator(IRepository repository)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (name, ct) => !await repository.ExistsAsync<BlazorApp.Domain.Account.Account>(
                a => a.Name == name, ct))
                .WithMessage((_, name) => string.Format("{0} account already exists", name));
    }
}