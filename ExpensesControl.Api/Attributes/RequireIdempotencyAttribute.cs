namespace ExpensesControl.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RequireIdempotencyAttribute : Attribute
    {
    }
}
