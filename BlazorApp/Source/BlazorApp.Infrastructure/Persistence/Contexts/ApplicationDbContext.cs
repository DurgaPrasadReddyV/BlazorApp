using System.Data;
using BlazorApp.Application.Common.Interfaces;
using BlazorApp.Application.Identity.Interfaces;
using BlazorApp.Domain.Common.Contracts;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Infrastructure.Persistence.Contexts;

public class ApplicationDbContext : BaseDbContext
{
    private readonly IEventService _eventService;
    public IDbConnection Connection => Database.GetDbConnection();
    private readonly ICurrentUser _currentUserService;

    public ApplicationDbContext(DbContextOptions options, ICurrentUser currentUserService, ISerializerService serializer, IEventService eventService)
    : base(options, currentUserService, serializer)
    {
        _currentUserService = currentUserService;
        _eventService = eventService;
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