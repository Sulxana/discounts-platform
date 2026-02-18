using Discounts.Application.Offers.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discounts.Infrastracture.BackgroundJobs
{
    public class OfferExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<OfferExpirationWorker> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);

        public OfferExpirationWorker(IServiceScopeFactory serviceScopeFactory, ILogger<OfferExpirationWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Offer Expiration Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessExpiration(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in Offer Expiration Worker.");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.LogInformation("Offer Expiration Worker stopping.");
        }

        private async Task ProcessExpiration(CancellationToken token)
        {
            List<Guid> expiredIds;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IOfferRepository>();
                var offers = await repo.GetExpiredActiveAsync(token);
                expiredIds = offers.Select(o => o.Id).ToList();
            }

            if (!expiredIds.Any()) return;

            _logger.LogInformation($"Found {expiredIds.Count} expired offers to process.");

            foreach (var id in expiredIds)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var service = scope.ServiceProvider.GetRequiredService<IOfferCleanupService>();
                        await service.ProcessOffer(id, token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Critical error processing offer {id} in worker.");
                    }
                }
            }
        }
    }
}
