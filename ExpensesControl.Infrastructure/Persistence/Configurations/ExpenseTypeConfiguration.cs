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
    public class ExpenseTypeConfiguration : IEntityTypeConfiguration<ExpenseType>
    {
        public void Configure(EntityTypeBuilder<ExpenseType> builder)
        {
            builder.ToTable("ExpenseTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.IsActive)
                .IsRequired();


            builder.HasMany(x => x.ExpenseDetails)
                .WithOne(d => d.ExpenseType)
                .HasForeignKey(d => d.ExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Budgets)
                .WithOne(b => b.ExpenseType)
                .HasForeignKey(b => b.ExpenseTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
