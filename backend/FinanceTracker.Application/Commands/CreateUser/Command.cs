namespace FinanceTracker.Application.Commands.CreateUser;

public sealed record Command(
    string Name,
    decimal InitialBalance,
    DateOnly StartDate,
    decimal SalaryAmount,
    int SalaryDayOfMonth);