using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(user => user.Id);
        
        builder.Property(user => user.Name)
            .IsRequired();

        builder.Property(user => user.InitialBalance)
            .HasColumnType("decimal(18,2)");

        builder.Property(user => user.StartDate)
            .HasColumnType("date");
        
        builder.HasMany(user => user.RecurringTransactions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(user => user.OneOffTransactions)
            .WithOne()
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}