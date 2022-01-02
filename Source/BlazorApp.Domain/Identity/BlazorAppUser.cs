using BlazorApp.Domain.MoneyTransfer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Domain.Identity
{
    internal class BlazorAppUser
    {
        public ICollection<Account> Accounts { get; set; }
    }
}
