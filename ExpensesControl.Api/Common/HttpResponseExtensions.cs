using System.Text.Json;

namespace ExpensesControl.Api.Common
{
    public static class HttpResponseExtensions
    {
        public static Task WriteApiErrorAsync(
            this HttpResponse response,
            int statusCode,
            ApiErrorResponse error,
            CancellationToken ct,
            string? retryAfterSeconds = null)
        {
            response.StatusCode = statusCode;
            response.ContentType = "application/json; charset=utf-8";

            if (!string.IsNullOrWhiteSpace(retryAfterSeconds))
                response.Headers["Retry-After"] = retryAfterSeconds;

            return response.WriteAsJsonAsync(error, new JsonSerializerOptions
            {
                PropertyNamingPolicy = null
            }, ct);
        }
    }
}
