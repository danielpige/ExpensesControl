using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Application.Common.Security
{
    public static class JwtOptions
    {
        public const string Key = "ThisIsAVeryLongJwtSecretKey_ChangeMeNow_123!";
        public const string Issuer = "ExpensesControl";
        public const string Audience = "ExpensesControlClient";
    }
}
