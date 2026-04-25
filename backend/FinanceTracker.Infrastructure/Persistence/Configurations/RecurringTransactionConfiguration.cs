using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class RecurringTransactionConfiguration : IEntityTypeConfiguration<RecurringTransaction>
{
    public void Configure(EntityTypeBuilder<RecurringTransaction> builder)
    {
        builder.ToTable("RecurringTransaction");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Type);

        builder.Property(x => x.Kind);

        builder.Property(x => x.ScheduledDayOfMonth);

        builder.Property(x => x.StartDate)
            .HasColumnType("date");

        builder.Property(x => x.EndDate)
            .HasColumnType("date");
        
        builder.HasOne<User>()
            .WithMany(user => user.RecurringTransactions)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}