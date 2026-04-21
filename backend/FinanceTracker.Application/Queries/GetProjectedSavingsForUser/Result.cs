namespace FinanceTracker.Application.Queries.GetProjectedSavingsForUser;

public sealed record Result(decimal ProjectedSavings, DateOnly CycleEndDate);