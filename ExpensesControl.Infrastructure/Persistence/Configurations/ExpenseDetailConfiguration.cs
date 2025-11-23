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
    public class ExpenseDetailConfiguration : IEntityTypeConfiguration<ExpenseDetail>
    {
        public void Configure(EntityTypeBuilder<ExpenseDetail> builder)
        {
            builder.ToTable("ExpenseDetails");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Comments)
                .HasMaxLength(500);

            builder.HasOne(x => x.ExpenseHeader)
                .WithMany(h => h.Details)
                .HasForeignKey(x => x.ExpenseHeaderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ExpenseType)
                .WithMany(t => t.ExpenseDetails)
                .HasForeignKey(x => x.ExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
