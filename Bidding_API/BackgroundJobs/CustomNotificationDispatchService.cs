using System;
using System.Threading;
using System.Threading.Tasks;
using Bidding_Core.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Bidding_API.BackgroundJobs
{
    /// <summary>Sends admin-authored custom notifications once their scheduled time has passed.</summary>
    public class CustomNotificationDispatchService : BackgroundService
    {
        private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(15);

        private readonly ILogger<CustomNotificationDispatchService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public CustomNotificationDispatchService(
            ILogger<CustomNotificationDispatchService> logger,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var service = scope.ServiceProvider.GetRequiredService<ICustomNotificationService>();
                        var processed = await service.ProcessDueAsync(DateTime.UtcNow);
                        if (processed > 0)
                        {
                            _logger.LogInformation(
                                "CustomNotificationDispatchService: processed {Count} notification(s) at {Time}.",
                                processed, DateTime.UtcNow);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in CustomNotificationDispatchService");
                }

                await Task.Delay(PollInterval, stoppingToken);
            }
        }
    }
}
