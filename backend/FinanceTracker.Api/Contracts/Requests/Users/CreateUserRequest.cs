namespace FinanceTracker.Api.Contracts.Requests.Users;

public sealed record CreateUserRequest(
    string Name,
    decimal InitialBalance,
    DateOnly StartDate,
    decimal SalaryAmount,
    int SalaryDayOfMonth);