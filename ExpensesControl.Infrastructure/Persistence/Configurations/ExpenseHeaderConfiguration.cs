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
    public class ExpenseHeaderConfiguration : IEntityTypeConfiguration<ExpenseHeader>
    {
        public void Configure(EntityTypeBuilder<ExpenseHeader> builder)
        {
            builder.ToTable("ExpenseHeaders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Date)
                .IsRequired();

            builder.Property(x => x.MerchantName)
                .HasMaxLength(150);

            builder.Property(x => x.Comments)
                .HasMaxLength(500);

            builder.Property(x => x.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DocumentType)
                .IsRequired();

            builder.HasOne(x => x.MoneyFund)
                .WithMany(f => f.Expenses)
                .HasForeignKey(x => x.MoneyFundId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.User)
                .WithMany(u => u.ExpenseHeaders)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
