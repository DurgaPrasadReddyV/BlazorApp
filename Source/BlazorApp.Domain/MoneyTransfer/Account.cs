namespace BlazorApp.Domain.MoneyTransfer
{
    public class Account
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public decimal Balance { get; set; }

        public string UniqueId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string UserId { get; set; }

        public ICollection<Transaction> Transfers { get; set; }
    }
}