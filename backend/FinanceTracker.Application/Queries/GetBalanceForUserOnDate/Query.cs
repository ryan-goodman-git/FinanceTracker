namespace FinanceTracker.Application.Queries.GetBalanceForUserOnDate;

public sealed record Query(Guid UserId, DateOnly TargetDate);