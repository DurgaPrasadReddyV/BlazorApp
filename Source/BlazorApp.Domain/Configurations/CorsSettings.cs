using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorApp.Domain.Configurations
{
    public class CorsSettings
    {
        public List<string>? AllowedOrigins { get; set; }
    }
}
