namespace ExpensesControl.Api.Common
{
    public class ApiPolicies
    {
        public const string CorsAngular = "AllowAngularClient";

        public static class RateLimiting
        {
            public const string AuthStrict = "auth_strict";
            public const string WritesUser = "writes_user";
            public const string ReadsUser = "reads_user";
            public const string ReportsUser = "reports_user";
            public const string ReportsConcurrency = "reports_concurrency";
            public const string ExportsUser = "exports_user";
        }
    }
}
