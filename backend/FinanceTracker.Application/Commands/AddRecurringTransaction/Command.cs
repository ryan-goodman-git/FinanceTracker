using FinanceTracker.Domain.Enums;

namespace FinanceTracker.Application.Commands.AddRecurringTransaction;

public sealed record Command(
    Guid UserId,
    string Description,
    decimal Amount,
    int ScheduledDayOfMonth,
    DateOnly StartDate);