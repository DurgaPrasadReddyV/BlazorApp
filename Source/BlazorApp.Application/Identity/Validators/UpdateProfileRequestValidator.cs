using BlazorApp.Application.Common.Validation;
using BlazorApp.Application.FileStorage;
using BlazorApp.Shared.Identity;
using FluentValidation;

namespace BlazorApp.Application.Identity.Validators;

public class UpdateProfileRequestValidator : CustomValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(p => p.FirstName).MaximumLength(75).NotEmpty();
        RuleFor(p => p.Email).NotEmpty();
        RuleFor(p => p.Image).SetNonNullableValidator(new FileUploadRequestValidator());
    }
}