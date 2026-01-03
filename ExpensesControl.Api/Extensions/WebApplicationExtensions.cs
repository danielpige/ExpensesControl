using ExpensesControl.Api.Common;
using ExpensesControl.Api.Middlewares;

namespace ExpensesControl.Api.Extensions
{
    public static class WebApplicationExtensions
    {
        public static WebApplication UseApiPipeline(this WebApplication app)
        {
            // Manejo de errores primero
            app.UseMiddleware<ErrorHandlingMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(ApiPolicies.CorsAngular);

            // Para que RateLimiter pueda usar userId (si dependemos de auth)
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            // Idempotencia típicamente después de RL (rechazas temprano si está limitado)
            app.UseMiddleware<IdempotencyMiddleware>();

            app.MapControllers();

            return app;
        }
    }
}
