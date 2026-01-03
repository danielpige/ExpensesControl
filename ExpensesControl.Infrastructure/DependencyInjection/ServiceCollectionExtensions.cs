using ExpensesControl.Application.Common.Interfaces;
using ExpensesControl.Application.Common.Interfaces.Services;
using ExpensesControl.Infrastructure.Persistence;
using ExpensesControl.Infrastructure.Repositories;
using ExpensesControl.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // DbContext
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // UnitOfWork + Repositories
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Application services (implementaciones reales)
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IMoneyFundService, MoneyFundService>();
            services.AddScoped<IExpenseService, ExpenseService>();
            services.AddScoped<IBudgetService, BudgetService>();
            services.AddScoped<IDepositService, DepositService>();
            services.AddScoped<IMovementService, MovementService>();
            services.AddScoped<IExpenseTypeService, ExpenseTypeService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<IMovementReportService, MovementReportService>();

            // Background cleanup (Idempotency)
            services.AddHostedService<IdempotencyCleanupService>();

            return services;
        }
    }
}
