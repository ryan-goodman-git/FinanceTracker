using FinanceTracker.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinanceTracker.Infrastructure.Persistence.Configurations;

public class OneOffTransactionConfiguration : IEntityTypeConfiguration<OneOffTransaction>
{
    public void Configure(EntityTypeBuilder<OneOffTransaction> builder)
    {
        builder.ToTable("OneOffTransaction");

        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .ValueGeneratedNever();
        
        builder.Property(x => x.Description)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Type);

        builder.Property(x => x.Date)
            .HasColumnType("date");
        
        builder.HasOne<User>()
            .WithMany(user => user.OneOffTransactions)
            .HasForeignKey("UserId")
            .OnDelete(DeleteBehavior.Cascade);
    }
}