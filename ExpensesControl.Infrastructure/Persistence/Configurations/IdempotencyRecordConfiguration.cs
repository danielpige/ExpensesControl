using ExpensesControl.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpensesControl.Infrastructure.Persistence.Configurations
{
    public class IdempotencyRecordConfiguration : IEntityTypeConfiguration<IdempotencyRecord>
    {
        public void Configure(EntityTypeBuilder<IdempotencyRecord> builder)
        {
            builder.ToTable("IdempotencyRecords");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Key).HasMaxLength(128).IsRequired();
            builder.Property(x => x.Scope).HasMaxLength(64).IsRequired();
            builder.Property(x => x.Method).HasMaxLength(10).IsRequired();
            builder.Property(x => x.Path).HasMaxLength(256).IsRequired();
            builder.Property(x => x.RequestHash).HasMaxLength(64).IsRequired();

            builder.Property(x => x.ContentType).HasMaxLength(128);

            // Índice único: evita que la misma operación se ejecute dos veces por carrera
            builder.HasIndex(x => new { x.Scope, x.Key, x.Method, x.Path })
             .IsUnique();

            // Índices útiles para limpieza
            builder.HasIndex(x => x.ExpiresAt);
        }
    }
}
