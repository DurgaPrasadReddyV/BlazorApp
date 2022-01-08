using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Validation;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorApp.Application.Account;

public class UpdateAccountRequestValidator : CustomValidator<UpdateAccountRequest>
{
    public UpdateAccountRequestValidator(IRepository repository)
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .MaximumLength(75)
            .MustAsync(async (account, name, ct) => !await repository.ExistsAsync<BlazorApp.Domain.Account.Account>(
                a => a.Id != account.Id && a.Name == name, ct))
                .WithMessage((_, name) => string.Format("{0} account already exists", name));
    }
}