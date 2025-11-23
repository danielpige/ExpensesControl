namespace ExpensesControl.Api.Common
{
    public class ApiErrorResponse
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = null!;

        public ApiErrorResponse(string message)
        {
            Message = message;
        }

        public static ApiErrorResponse Fail(string message)
            => new(message);
    }
}
