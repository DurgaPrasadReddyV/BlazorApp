using BlazorApp.Application.Common.Validation;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Shared.Identity;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Application.Identity.Validators
{
    public class RoleRequestValidator : CustomValidator<RoleRequest>
    {
        private readonly IRoleService _roleService = default!;

        public RoleRequestValidator(IRoleService roleService)
        {
            _roleService = roleService;

            RuleFor(r => r.Name)
                .NotEmpty()
                .MustAsync(async (role, name, _) => !await _roleService.ExistsAsync(name!, role.Id))
                    .WithMessage("Similar Role already exists.");
        }
    }
}
