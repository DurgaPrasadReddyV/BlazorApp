using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Host.Entities
{
    public class BlazorAppUser : IdentityUser<Guid>
    {
    }
}
