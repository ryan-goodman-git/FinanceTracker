using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Domain.Entities;

public class OneOffTransaction
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateOnly Date { get; private set; }

    private OneOffTransaction()
    {
    }

    public OneOffTransaction(
        Guid id,
        Guid userId,
        string description,
        decimal amount,
        TransactionType type,
        DateOnly date)
    {
        if (id == Guid.Empty) throw new ArgumentException("Transaction id cannot be empty.", nameof(id));
        if (userId == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(userId));
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");

        Id = id;
        UserId = userId;
        Description = description.Trim();
        Amount = amount;
        Type = type;
        Date = date;
    }

    public void UpdateDetails(string description, decimal amount)
    {
        if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Description is required.", nameof(description));
        
        if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        
        Description = description.Trim();
        Amount = amount;
    }
}