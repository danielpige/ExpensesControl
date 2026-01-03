using ExpensesControl.Api.Common;
using System.Threading.RateLimiting;

namespace ExpensesControl.Api.DependencyInjection
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddApiRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.ContentType = "application/json; charset=utf-8";

                    if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                    {
                        var seconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
                        context.HttpContext.Response.Headers["Retry-After"] = seconds.ToString();
                    }

                    await context.HttpContext.Response.WriteAsJsonAsync(
                        ApiErrorResponse.Fail("Too many requests. Please try again later."),
                        cancellationToken: token);
                };

                // Global limiter (suave): por IP (o por usuario si autenticado)
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value;

                    var key = userId ?? (httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip");

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: key,
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 120,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });
                // Para endpoints publicos
                options.AddPolicy(ApiPolicies.RateLimiting.AuthStrict, httpContext =>
                {
                    var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown-ip";

                    return RateLimitPartition.GetTokenBucketLimiter(ip, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 5,
                        TokensPerPeriod = 5,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        AutoReplenishment = true,
                        QueueLimit = 0
                    });
                });
                // Para endpoints que modifican datos como (POST, PUT, PATCH, DELETE)
                options.AddPolicy(ApiPolicies.RateLimiting.WritesUser, httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(userId, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 30,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    });
                });
                // Para endpoints de lectura basicos con consultas simples
                options.AddPolicy(ApiPolicies.RateLimiting.ReadsUser, httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? (httpContext.Connection.RemoteIpAddress?.ToString() ?? "anon");

                    return RateLimitPartition.GetSlidingWindowLimiter(userId, _ => new SlidingWindowRateLimiterOptions
                    {
                        PermitLimit = 300,
                        Window = TimeSpan.FromMinutes(1),
                        SegmentsPerWindow = 6,
                        QueueLimit = 0
                    });
                });
                // Para endpoints de reportes y con consultas un poco más pesadas
                options.AddPolicy(ApiPolicies.RateLimiting.ReportsUser, httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? "anonymous";

                    return RateLimitPartition.GetTokenBucketLimiter(userId, _ => new TokenBucketRateLimiterOptions
                    {
                        TokenLimit = 10,
                        TokensPerPeriod = 10,
                        ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                        AutoReplenishment = true,
                        QueueLimit = 0
                    });
                });
                // Para endpoints mas pesados que toman mas tiempo de carga
                options.AddPolicy(ApiPolicies.RateLimiting.ReportsConcurrency, httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? "anonymous";

                    return RateLimitPartition.GetConcurrencyLimiter(userId, _ => new ConcurrencyLimiterOptions
                    {
                        PermitLimit = 1,
                        QueueLimit = 0
                    });
                });
                // Para endpoints que generan reportes o tareas grandes
                options.AddPolicy(ApiPolicies.RateLimiting.ExportsUser, httpContext =>
                {
                    var userId = httpContext.User.FindFirst("userId")?.Value
                              ?? httpContext.User.FindFirst("sub")?.Value
                              ?? "anonymous";

                    return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 3,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    });
                });
            });

            return services;
        }
    }
}
