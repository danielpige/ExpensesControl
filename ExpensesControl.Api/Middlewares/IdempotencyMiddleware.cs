using ExpensesControl.Api.Attributes;
using ExpensesControl.Api.Common;
using ExpensesControl.Domain.Entities;
using ExpensesControl.Domain.Enums;
using ExpensesControl.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace ExpensesControl.Api.Middlewares
{
    public class IdempotencyMiddleware
    {
        private const int MaxResponseBytesToStore = 256 * 1024; // 256KB
        private static readonly TimeSpan DefaultTtl = TimeSpan.FromMinutes(10);

        private readonly RequestDelegate _next;

        public IdempotencyMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext ctx, AppDbContext db)
        {
            // 0) Solo aplica a endpoints marcados con [RequireIdempotency]
            var endpoint = ctx.GetEndpoint();
            var requires = endpoint?.Metadata.GetMetadata<RequireIdempotencyAttribute>() is not null;

            if (!requires)
            {
                await _next(ctx);
                return;
            }

            // 1) Validar header Idempotency-Key
            if (!ctx.Request.Headers.TryGetValue("Idempotency-Key", out var keyValues))
            {
                await WriteApiErrorAsync(ctx, StatusCodes.Status400BadRequest,
                    ApiErrorResponse.Fail("Missing Idempotency-Key header."));
                return;
            }

            var key = keyValues.ToString().Trim();
            if (string.IsNullOrWhiteSpace(key) || key.Length > 128)
            {
                await WriteApiErrorAsync(ctx, StatusCodes.Status400BadRequest,
                    ApiErrorResponse.Fail("Invalid Idempotency-Key."));
                return;
            }

            var method = ctx.Request.Method;
            var path = ctx.Request.Path.Value ?? string.Empty;

            // 2) Scope por usuario/tenant (ajusta a tus claims reales)
            var scope = ctx.User.FindFirst("userId")?.Value
                        ?? ctx.User.FindFirst("sub")?.Value
                        ?? "anonymous";

            // 3) Hash de la request (método + path + query + body)
            var requestHash = await ComputeRequestHashAsync(ctx);

            // 4) Revisar si ya existe un registro para (scope, key, method, path)
            var existing = await db.IdempotencyRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r =>
                    r.Scope == scope &&
                    r.Key == key &&
                    r.Method == method &&
                    r.Path == path,
                    ctx.RequestAborted);

            if (existing is not null)
            {
                // 4.1) Misma key usada con payload distinto => 409
                if (existing.RequestHash != requestHash)
                {
                    await WriteApiErrorAsync(ctx, StatusCodes.Status409Conflict,
                        ApiErrorResponse.Fail("Idempotency-Key was already used with a different request payload."));
                    return;
                }

                // 4.2) Aún procesando => 202 + Retry-After (puedes responder con tu envelope)
                if (existing.State == IdempotencyState.InProgress && existing.ExpiresAt > DateTime.UtcNow)
                {
                    await WriteApiErrorAsync(ctx, StatusCodes.Status202Accepted,
                        ApiErrorResponse.Fail("Request is being processed. Please retry."),
                        retryAfterSeconds: "1");
                    return;
                }

                // 4.3) Completada y vigente => replay (mismo status/body)
                if (existing.State == IdempotencyState.Completed && existing.ExpiresAt > DateTime.UtcNow)
                {
                    ctx.Response.StatusCode = existing.StatusCode ?? StatusCodes.Status200OK;

                    if (!string.IsNullOrWhiteSpace(existing.ContentType))
                        ctx.Response.ContentType = existing.ContentType;

                    if (existing.ResponseBody is { Length: > 0 })
                        await ctx.Response.Body.WriteAsync(existing.ResponseBody, ctx.RequestAborted);

                    return;
                }

                // Si expiró, se trata como “nuevo” (continúa)
            }

            // 5) Crear registro InProgress (protege concurrencia con índice único)
            var record = new IdempotencyRecord
            {
                Key = key,
                Scope = scope,
                Method = method,
                Path = path,
                RequestHash = requestHash,
                State = IdempotencyState.InProgress,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.Add(DefaultTtl)
            };

            try
            {
                db.IdempotencyRecords.Add(record);
                await db.SaveChangesAsync(ctx.RequestAborted);
            }
            catch (DbUpdateException)
            {
                // Carrera: otro request insertó primero.
                // Reconsultar y decidir: InProgress => 202, Completed => replay, Hash mismatch => 409
                var raced = await db.IdempotencyRecords
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r =>
                        r.Scope == scope &&
                        r.Key == key &&
                        r.Method == method &&
                        r.Path == path,
                        ctx.RequestAborted);

                if (raced is null)
                {
                    await WriteApiErrorAsync(ctx, StatusCodes.Status409Conflict,
                        ApiErrorResponse.Fail("Idempotency conflict. Please retry."));
                    return;
                }

                if (raced.RequestHash != requestHash)
                {
                    await WriteApiErrorAsync(ctx, StatusCodes.Status409Conflict,
                        ApiErrorResponse.Fail("Idempotency-Key was already used with a different request payload."));
                    return;
                }

                if (raced.State == IdempotencyState.Completed && raced.ExpiresAt > DateTime.UtcNow)
                {
                    ctx.Response.StatusCode = raced.StatusCode ?? StatusCodes.Status200OK;

                    if (!string.IsNullOrWhiteSpace(raced.ContentType))
                        ctx.Response.ContentType = raced.ContentType;

                    if (raced.ResponseBody is { Length: > 0 })
                        await ctx.Response.Body.WriteAsync(raced.ResponseBody, ctx.RequestAborted);

                    return;
                }

                await WriteApiErrorAsync(ctx, StatusCodes.Status202Accepted,
                    ApiErrorResponse.Fail("Request is being processed. Please retry."),
                    retryAfterSeconds: "1");
                return;
            }

            // 6) Capturar respuesta del endpoint para guardarla y poder “replay”
            var originalBody = ctx.Response.Body;
            await using var buffer = new MemoryStream();
            ctx.Response.Body = buffer;

            try
            {
                await _next(ctx);

                var statusCode = ctx.Response.StatusCode;

                // 6.1) Si 5xx, no se marca Completed: se borra registro para permitir retry
                if (statusCode >= 500)
                {
                    await db.IdempotencyRecords
                        .Where(r => r.Id == record.Id)
                        .ExecuteDeleteAsync(ctx.RequestAborted);

                    buffer.Position = 0;
                    await buffer.CopyToAsync(originalBody, ctx.RequestAborted);
                    return;
                }

                // 6.2) Guardar respuesta (limitando tamaño)
                var bodyBytes = buffer.ToArray();
                if (bodyBytes.Length > MaxResponseBytesToStore)
                    bodyBytes = Array.Empty<byte>();

                var contentType = ctx.Response.ContentType;

                await db.IdempotencyRecords
                    .Where(r => r.Id == record.Id)
                    .ExecuteUpdateAsync(setters => setters
                        .SetProperty(r => r.State, IdempotencyState.Completed)
                        .SetProperty(r => r.StatusCode, statusCode)
                        .SetProperty(r => r.ContentType, contentType)
                        .SetProperty(r => r.ResponseBody, bodyBytes)
                        .SetProperty(r => r.CompletedAt, DateTime.UtcNow),
                        ctx.RequestAborted);

                // 6.3) Escribir la respuesta real al cliente
                buffer.Position = 0;
                await buffer.CopyToAsync(originalBody, ctx.RequestAborted);
            }
            catch
            {
                // Si explota el endpoint, borrar para permitir retry con misma key
                await db.IdempotencyRecords
                    .Where(r => r.Id == record.Id)
                    .ExecuteDeleteAsync(ctx.RequestAborted);

                throw;
            }
            finally
            {
                ctx.Response.Body = originalBody;
            }
        }

        private static async Task<string> ComputeRequestHashAsync(HttpContext ctx)
        {
            // method + path + query + body
            ctx.Request.EnableBuffering();

            using var sha = SHA256.Create();
            var meta = $"{ctx.Request.Method}|{ctx.Request.Path}|{ctx.Request.QueryString}";
            var metaBytes = Encoding.UTF8.GetBytes(meta);

            await using var ms = new MemoryStream();
            await ms.WriteAsync(metaBytes);

            if (ctx.Request.ContentLength is > 0)
            {
                ctx.Request.Body.Position = 0;
                await ctx.Request.Body.CopyToAsync(ms);
                ctx.Request.Body.Position = 0;
            }

            ms.Position = 0;
            var hash = sha.ComputeHash(ms);
            return Convert.ToHexString(hash);
        }

        private static Task WriteApiErrorAsync(
            HttpContext ctx,
            int statusCode,
            ApiErrorResponse error,
            string? retryAfterSeconds = null)
        {
            ctx.Response.StatusCode = statusCode;
            ctx.Response.ContentType = "application/json; charset=utf-8";

            if (!string.IsNullOrWhiteSpace(retryAfterSeconds))
                ctx.Response.Headers["Retry-After"] = retryAfterSeconds;

            return ctx.Response.WriteAsJsonAsync(error, ctx.RequestAborted);
        }
    
    }
}
