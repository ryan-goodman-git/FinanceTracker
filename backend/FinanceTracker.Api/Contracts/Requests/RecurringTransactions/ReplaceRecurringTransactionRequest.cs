namespace FinanceTracker.Api.Contracts.Requests.RecurringTransactions;

public sealed record ReplaceRecurringTransactionRequest(
    string Description,
    decimal Amount,
    int ScheduledDayOfMonth,
    DateOnly ReplacementStartDate);