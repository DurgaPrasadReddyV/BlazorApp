using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Domain.Identity;

namespace BlazorApp.Domain.Account
{
    public class Account : AuditableEntity
    {
        public string? Name { get; set; }

        public decimal Balance { get; set; }

        public Guid BlazorAppUserId { get; set; }

        public virtual BlazorAppUser BlazorAppUser { get; set; } = default!;

        public string? UniqueId { get; set; }

        public ICollection<Transaction.Transaction>? Transfers { get; set; }

        public Account Update()
        {
            return null;
        }
    }
}