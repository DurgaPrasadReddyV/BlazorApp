using System.Linq.Expressions;
using BlazorApp.Application.Account.Interfaces;
using BlazorApp.Application.Common.Exceptions;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Common.Specifications;
using BlazorApp.Application.Wrapper;
using BlazorApp.Domain.Account;
using BlazorApp.Domain.Account.Events;
using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Domain.Dashboard;
using BlazorApp.Shared.Account;
using Microsoft.Extensions.Localization;

namespace DN.WebApi.Application.Catalog.Services;

public class AccountService : IAccountService
{
    private readonly IRepository _repository;

    public AccountService(IRepository repository, IStringLocalizer<AccountService> localizer)
    {
        _repository = repository;
    }

    public async Task<Result<Guid>> CreateAccountAsync(CreateAccountRequest request)
    {
        bool accountExists = await _repository.ExistsAsync<Account>(a => a.Name == request.Name);
        if (accountExists) 
            throw new EntityAlreadyExistsException(string.Format("Account {0} already exists", request.Name));

        var account = new Account();

        // Add Domain Events to be raised after the commit
        account.DomainEvents.Add(new AccountCreatedEvent(account));
        account.DomainEvents.Add(new StatsChangedEvent());

        var accountId = await _repository.CreateAsync(account);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(accountId);
    }

    public async Task<Result<Guid>> UpdateAccountAsync(UpdateAccountRequest request, Guid id)
    {
        var account = await _repository.GetByIdAsync<Account>(id, null);
        if (account == null) 
            throw new EntityNotFoundException(string.Format("Account {0} not found", id));

        var updatedAccount = account.Update();

        // Add Domain Events to be raised after the commit
        account.DomainEvents.Add(new AccountUpdatedEvent(account));
        account.DomainEvents.Add(new StatsChangedEvent());

        await _repository.UpdateAsync(updatedAccount);
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<Guid>> DeleteAccountAsync(Guid id)
    {
        var productToDelete = await _repository.RemoveByIdAsync<Account>(id);
        productToDelete.DomainEvents.Add(new AccountDeletedEvent(productToDelete));
        productToDelete.DomainEvents.Add(new StatsChangedEvent());
        await _repository.SaveChangesAsync();
        return await Result<Guid>.SuccessAsync(id);
    }

    public async Task<Result<AccountDetailsDto>> GetAccountDetailsAsync(Guid id)
    {
        var includes = new Expression<Func<Account, object>>[] { x => x.BlazorAppUser };
        var product = await _repository.GetByIdAsync<Account, AccountDetailsDto>(id, includes);
        return await Result<AccountDetailsDto>.SuccessAsync(product);
    }

    public async Task<PaginatedResult<AccountDto>> SearchAsync(AccountListFilter filter)
    {
        var filters = new Filters<Account>();

        var specification = new PaginationSpecification<Account>
        {
            AdvancedSearch = filter.AdvancedSearch,
            Filters = filters,
            Keyword = filter.Keyword,
            OrderBy = x => x.OrderBy(b => b.Name),
            OrderByStrings = filter.OrderBy,
            PageIndex = filter.PageNumber,
            PageSize = filter.PageSize,
            Includes = new Expression<Func<Account, object>>[] { x => x.BlazorAppUser }
        };

        return await _repository.GetListAsync<Account, AccountDto>(specification);
    }
}