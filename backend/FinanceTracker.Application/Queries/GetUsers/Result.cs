namespace FinanceTracker.Application.Queries.GetUsers;

public sealed record Result(
    Guid UserId,
    string Name,
    decimal InitialBalance,
    DateOnly StartDate);
