using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExpensesControl.Api.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddApiCors(configuration)
                .AddApiControllers()
                .AddApiFluentValidation()
                .AddApiRateLimiting()
                .AddApiSwagger()
                .AddApiAuthentication(configuration)
                .AddAuthorization();

            return services;
        }
    }
}
