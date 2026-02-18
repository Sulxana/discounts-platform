using Discounts.Application.Reservations.Interfaces;
using Discounts.Application.Reservations.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Discounts.Infrastracture.BackgroundJobs
{
    public class ReservationCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<ReservationCleanupWorker> _logger;

        // Default interval 
        private readonly TimeSpan _defaultInterval = TimeSpan.FromMinutes(1);

        public ReservationCleanupWorker(IServiceScopeFactory serviceScopeFactory,ILogger<ReservationCleanupWorker> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Reservation Cleanup Worker started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessCleanup(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while processing reservation cleanup.");
                }
                
                await Task.Delay(_defaultInterval, stoppingToken);
            }

            _logger.LogInformation("Reservation Cleanup Worker stopping.");
        }

        private async Task ProcessCleanup(CancellationToken token)
        {
            List<Guid> expiredIds;

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<IReservationRepository>();
                var reservations = await repo.GetExpiredActiveAsync(token);
                expiredIds = reservations.Select(r => r.Id).ToList();
            }

            if (!expiredIds.Any()) return;
            
            _logger.LogInformation($"Found {expiredIds.Count} expired reservations. Processing individually...");

            foreach (var id in expiredIds)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    try
                    {
                        var cleanupService = scope.ServiceProvider.GetRequiredService<IReservationCleanupService>();
                        await cleanupService.ProcessReservation(id, token);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Critical error processing reservation {id} in worker.");
                    }
                }
            }
        }
    }
}
