using BlazorApp.CommonInfrastructure.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppUsers
    {
        public static IEnumerable<BlazorAppUser> Get()
        {
            return new List<BlazorAppUser>
            {
                new BlazorAppUser
                {
                    FirstName = "Administrator",
                    UserName = "admin",
                    Email = "admin@blazorapp.com",
                    EmailConfirmed = true,
                    IsActive = true
                }
            };
        }
    }
}
