namespace FinanceTracker.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public decimal InitialBalance { get; private set; }
    public DateOnly StartDate { get; private set; }

    private readonly List<RecurringTransaction> _recurringTransactions = new();
    public IReadOnlyCollection<RecurringTransaction> RecurringTransactions => _recurringTransactions;

    private readonly List<OneOffTransaction> _oneOffTransactions = new();
    public IReadOnlyCollection<OneOffTransaction> OneOffTransactions => _oneOffTransactions;

    private User()
    {
    }

    public User(Guid id, string name, decimal initialBalance, DateOnly startDate)
    {
        if (id == Guid.Empty) throw new ArgumentException("User id cannot be empty.", nameof(id));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("User name is required.", nameof(name));

        Id = id;
        Name = name.Trim();
        InitialBalance = initialBalance;
        StartDate = startDate;
    }

    public void AddRecurringTransaction(RecurringTransaction recurringTransaction)
    {
        _recurringTransactions.Add(recurringTransaction);
    }

    public void AddOneOffTransaction(OneOffTransaction oneOffTransaction)
    {
        _oneOffTransactions.Add(oneOffTransaction);
    }
}