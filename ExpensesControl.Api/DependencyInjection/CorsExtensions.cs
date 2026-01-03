using ExpensesControl.Api.Common;

namespace ExpensesControl.Api.DependencyInjection
{
    public static class CorsExtensions
    {
        public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
        {
            // Recomendado: leer desde configuración
            // appsettings.json: "Cors": { "AllowedOrigins": ["http://localhost:4200"] }
            var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                          ?? new[] { "http://localhost:4200" };

            services.AddCors(options =>
            {
                options.AddPolicy(ApiPolicies.CorsAngular, policy =>
                {
                    policy.WithOrigins(origins)
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }
    }
}
