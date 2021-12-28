using BlazorApp.CommonInfrastructure.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.DbMigrator.Identity.SeedData
{
    internal class BlazorAppRoles
    {
        public static IEnumerable<BlazorAppRole> Get()
        {
            return new List<BlazorAppRole>
            {
                new BlazorAppRole
                {
                    Name = "Admin",
                    Description = "Admin Role"
                }
            };
        }
    }
}
