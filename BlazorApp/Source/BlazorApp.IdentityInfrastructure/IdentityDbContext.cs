using System.Data;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.CommonInfrastructure.Identity.Models;
using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.CommonInfrastructure.Persistence.Contexts;

public class IdentityDbContext : IdentityDbContext<BlazorAppUser, BlazorAppRole, string, BlazorAppUserClaim, BlazorAppUserRole, BlazorAppUserLogin, BlazorAppRoleClaim, BlazorAppUserToken>
{
    private readonly IEventService _eventService;
    private readonly ICurrentUser _currentUserService;

    public IdentityDbContext(DbContextOptions options, ICurrentUser currentUserService, IEventService eventService)
    : base(options)
    {
        _currentUserService = currentUserService;
        _eventService = eventService;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BlazorAppRole>().ToTable(TableNames.BlazorAppRoles);
        modelBuilder.Entity<BlazorAppRoleClaim>().ToTable(TableNames.BlazorAppRoleClaims);
        modelBuilder.Entity<BlazorAppUserRole>().ToTable(TableNames.BlazorAppUserRoles);
        modelBuilder.Entity<BlazorAppUser>().ToTable(TableNames.BlazorAppUsers);
        modelBuilder.Entity<BlazorAppUserLogin>().ToTable(TableNames.BlazorAppUserLogins);
        modelBuilder.Entity<BlazorAppUserClaim>().ToTable(TableNames.BlazorAppUserClaims);
        modelBuilder.Entity<BlazorAppUserToken>().ToTable(TableNames.BlazorAppUserTokens);

        modelBuilder.AppendGlobalQueryFilter<ISoftDelete>(s => s.DeletedOn == null);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.EnableSensitiveDataLogging();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentUserId = _currentUserService.GetUserId();
        foreach (var entry in ChangeTracker.Entries<IAuditableEntity>().ToList())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = currentUserId;
                    entry.Entity.LastModifiedBy = currentUserId;
                    break;

                case EntityState.Modified:
                    entry.Entity.LastModifiedOn = DateTime.UtcNow;
                    entry.Entity.LastModifiedBy = currentUserId;
                    break;

                case EntityState.Deleted:
                    if (entry.Entity is ISoftDelete softDelete)
                    {
                        softDelete.DeletedBy = currentUserId;
                        softDelete.DeletedOn = DateTime.UtcNow;
                        entry.State = EntityState.Modified;
                    }

                    break;
            }
        }

        int results = await base.SaveChangesAsync(cancellationToken);

        if (_eventService == null) return results;
        var entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
                                                .Select(e => e.Entity)
                                                .Where(e => e.DomainEvents.Count > 0)
                                                .ToArray();

        foreach (var entity in entitiesWithEvents)
        {
            var events = entity.DomainEvents.ToArray();
            entity.DomainEvents.Clear();
            foreach (var @event in events)
            {
                await _eventService.PublishAsync(@event);
            }
        }

        return results;
    }
}