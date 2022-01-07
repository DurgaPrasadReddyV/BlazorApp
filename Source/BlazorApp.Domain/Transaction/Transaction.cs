﻿using BlazorApp.Domain.Common.Contracts;

namespace BlazorApp.Domain.Transaction
{
    public class Transaction : AuditableEntity
    {
        public string? Description { get; set; }

        public decimal Amount { get; set; }

        public DateTime MadeOn { get; set; }

        public string? AccountId { get; set; }

        public string? Source { get; set; }

        public string? SenderName { get; set; }

        public string? RecipientName { get; set; }

        public string? Destination { get; set; }

        public string? ReferenceNumber { get; set; }
    }
}