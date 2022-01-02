using BlazorApp.Domain.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Domain.Identity
{
    public class BlazorAppUser : AuditableEntity
    {
        public Guid IdentityUserId { get; set; }

        public ICollection<Account.Account>? Accounts { get; set; }
    }
}
