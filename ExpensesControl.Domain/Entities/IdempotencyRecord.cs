using ExpensesControl.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Domain.Entities
{
    public class IdempotencyRecord
    {
        public long Id { get; set; }
        public string Key { get; set; } = default!;
        public string Scope { get; set; } = default!;     // userId/tenantId/companyId
        public string Method { get; set; } = default!;
        public string Path { get; set; } = default!;
        public string RequestHash { get; set; } = default!;
        public IdempotencyState State { get; set; }
        public int? StatusCode { get; set; }
        public string? ContentType { get; set; }
        public byte[]? ResponseBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
