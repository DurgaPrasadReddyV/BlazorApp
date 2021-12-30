using BlazorApp.Application.Common.Validation;
using BlazorApp.Shared.Identity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Application.Identity.Validators
{
    public class ChangePasswordRequestValidator : CustomValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator()
        {
            RuleFor(p => p.Password).NotEmpty();
            RuleFor(p => p.NewPassword).NotEmpty();
            RuleFor(p => p.ConfirmNewPassword).Equal(p => p.NewPassword).WithMessage("Passwords do not match.");
        }
    }
}
