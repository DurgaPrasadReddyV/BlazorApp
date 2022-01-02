using BlazorApp.Application.Wrapper;
using BlazorApp.Shared.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Application.Account.Interfaces
{
    public interface IAccountService
    {
        Task<Result<AccountDetailsDto>> GetAccountDetailsAsync(Guid id);

        Task<PaginatedResult<AccountDto>> SearchAsync(AccountListFilter filter);

        Task<Result<Guid>> CreateAccountAsync(CreateAccountRequest request);

        Task<Result<Guid>> UpdateAccountAsync(UpdateAccountRequest request, Guid id);

        Task<Result<Guid>> DeleteAccountAsync(Guid id);
    }
}
