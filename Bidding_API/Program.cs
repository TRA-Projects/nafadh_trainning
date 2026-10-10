using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Bidding_API
{
    public class Program
    {
        public static void Main(string[] args)
        {
           // CreateHostBuilder(args).Build().Run();
            CreateHostBuilder(args).Build().Run();

          //  var logger = NLog.Web.NLogBuilder.ConfigureNLog("Nlog.config").GetCurrentClassLogger();
            //try
            //{
            //    logger.Debug("init main");
            //    CreateHostBuilder(args).Build().Run();
            //}
            //catch (Exception exception)
            //{
            //    //NLog: catch setup errors
            //    logger.Error(exception, "Stopped program because of exception");
            //    throw;
            //}
            //finally
            //{
            //    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
            //    NLog.LogManager.Shutdown();
            //}
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                #region Get Environment App Settings
                var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                string filename = String.Format("appsettings.{0}.json", env);
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile(filename, optional: true, reloadOnChange: true); // optional extra provider
                config.AddEnvironmentVariables();
                #endregion

                #region Show File Name in EventViwer
                if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Windows))
                {
                    using (EventLog eventLog = new EventLog("Application"))
                    {
                        eventLog.Source = "Application";
                        eventLog.WriteEntry("API = " + filename, EventLogEntryType.Information, 101, 1);
                    }
                }
                #endregion
            })
            .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));

                    //logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Error);
                });
    }
}
