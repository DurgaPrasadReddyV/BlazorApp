using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Transaction
{
    public class Transaction : AuditableEntity
    {
        public Transaction(string name)
        {
            Name = name;
        }

        internal Transaction()
        {
        
        }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime MadeOn { get; set; }

        public Guid AccountId { get; set; }

        public Account.Account Account { get; set; }

        public string? Source { get; set; }

        public string? SenderName { get; set; }

        public string? RecipientName { get; set; }

        public string? Destination { get; set; }

        public string? ReferenceNumber { get; set; }
    }
}