using FluentValidation.AspNetCore;

namespace ExpensesControl.Api.DependencyInjection
{
    public static class ValidationExtensions
    {
        public static IServiceCollection AddApiFluentValidation(this IServiceCollection services)
        {
            services.AddFluentValidationAutoValidation();
            return services;
        }
    }
}
