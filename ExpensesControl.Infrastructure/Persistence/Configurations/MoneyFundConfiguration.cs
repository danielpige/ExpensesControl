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
    public class MoneyFundConfiguration : IEntityTypeConfiguration<MoneyFund>
    {
        public void Configure(EntityTypeBuilder<MoneyFund> builder)
        {
            builder.ToTable("MoneyFunds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.AccountType)
                .HasMaxLength(50);

            builder.Property(x => x.CurrentBalance)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.HasMany(x => x.Expenses)
                .WithOne(x => x.MoneyFund)
                .HasForeignKey(x => x.MoneyFundId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Deposits)
                .WithOne(x => x.MoneyFund)
                .HasForeignKey(x => x.MoneyFundId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
