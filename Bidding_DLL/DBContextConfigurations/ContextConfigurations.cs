using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Bidding_DLL.DBContextConfigurations
{
    public class ContextConfigurations
    {
        public string SQLConnectionString { get; set; }
        public ContextConfigurations()
        {
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            Debug.WriteLine(environmentName);
            var configBuilder =new ConfigurationBuilder();
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json" );
            configBuilder.AddJsonFile(path, true,true);
            var root = configBuilder.Build();

            var appsettings = root.GetSection("ConnectionStrings:SqlConnection");
            SQLConnectionString = appsettings.Value;
        }

      
    }
}
