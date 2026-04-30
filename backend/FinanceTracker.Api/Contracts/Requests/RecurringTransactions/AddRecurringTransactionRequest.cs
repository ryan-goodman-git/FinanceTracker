namespace FinanceTracker.Api.Contracts.Requests.RecurringTransactions;

public sealed record AddRecurringTransactionRequest(
    string Description,
    decimal Amount,
    int ScheduledDayOfMonth,
    DateOnly StartDate);