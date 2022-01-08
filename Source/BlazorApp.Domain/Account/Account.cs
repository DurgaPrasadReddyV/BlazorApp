using BlazorApp.Domain.Common.Contracts;
using BlazorApp.Domain.Identity;

namespace BlazorApp.Domain.Account
{
    public class Account : AuditableEntity
    {
        public Account(string name, Guid userId)
        {
            Name = name;
            BlazorAppUserId = userId;
        }

        internal Account()
        {
        }

        public Account Update(string name)
        {
            Name = name;
            return this;
        }

        public string? Name { get; set; }

        public decimal Balance { get; set; }

        public Guid BlazorAppUserId { get; set; }

        public BlazorAppUser BlazorAppUser { get; set; }

        public ICollection<Transaction.Transaction>? Transfers { get; set; }
    }
}