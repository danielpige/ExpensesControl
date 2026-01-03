using ExpensesControl.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

namespace ExpensesControl.Infrastructure.Services
{
    public class IdempotencyCleanupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public IdempotencyCleanupService(IServiceScopeFactory scopeFactory)
            => _scopeFactory = scopeFactory;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);

                using var scope = _scopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var now = DateTime.UtcNow;
                await db.IdempotencyRecords
                    .Where(x => x.ExpiresAt <= now)
                    .ExecuteDeleteAsync(stoppingToken);
            }
        }
    }
}
