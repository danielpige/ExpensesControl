namespace ExpensesControl.Api.DependencyInjection
{
    public static class ControllersExtensions
    {
        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(opt =>
                {
                    // Mantener PascalCase. Cambiar a camelCase si es necesario.
                    opt.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            return services;
        }
    }
}
