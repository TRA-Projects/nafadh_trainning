using Bidding_DLL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bidding_DLL.DBContextConfigurations
{
    public class ContextFactory : IDesignTimeDbContextFactory<CHDBContext>
    {
        public CHDBContext CreateDbContext(string[] args)
        {
            ContextConfigurations appconfig = new ContextConfigurations();
            var opBuilder = new DbContextOptionsBuilder<CHDBContext>();
            opBuilder.UseSqlServer(appconfig.SQLConnectionString);
            return new CHDBContext(opBuilder.Options);
        }
    }
}
