using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Enums;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var user = User.Create(
    Guid.NewGuid(),
    "Ryan",
    1000m,
    new DateOnly(2026, 4, 1),
    2000m,
    15);

var gym = new RecurringTransaction(
    Guid.NewGuid(),
    user.Id,
    "Gym",
    50m,
    TransactionType.Expense,
    RecurringTransactionKind.Expense,
    new DateOnly(2026, 4, 1),
    null,
    5);

user.AddRecurringTransaction(gym);
user.EndRecurringTransaction(gym.Id, new DateOnly(2026, 4, 30));

try
{
    user.ReplaceRecurringTransaction(
        gym.Id,
        "Gym",
        70m,
        5,
        new DateOnly(2026, 6, 1));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

/*
var user = User.Create(
    Guid.NewGuid(),
    "Ryan",
    1000m,
    new DateOnly(2026, 4, 1),
    2000m,
    15);

// Original gym transaction: £50 on the 5th, starting in April
var gym = new RecurringTransaction(
    Guid.NewGuid(),
    user.Id,
    "Gym",
    50m,
    TransactionType.Expense,
    RecurringTransactionKind.Expense,
    new DateOnly(2026, 4, 1),
    null,
    5);

user.AddRecurringTransaction(gym);

// Replace gym from 1 May with a new £70 version
user.ReplaceRecurringTransaction(
    gym.Id,
    "Gym",
    70m,
    5,
    new DateOnly(2026, 5, 1));

// Get the replacement gym transaction so we can end it later
var replacementGym = user.RecurringTransactions
    .Single(t =>
        t.Description == "Gym" &&
        t.Amount == 70m &&
        t.StartDate == new DateOnly(2026, 5, 1));

// End the replacement version on 31 May
user.EndRecurringTransaction(
    replacementGym.Id,
    new DateOnly(2026, 5, 31));

// Inspect recurring transactions
Console.WriteLine("Recurring transactions:");
foreach (var t in user.RecurringTransactions.OrderBy(t => t.StartDate))
{
    Console.WriteLine($"{t.Description} | {t.Amount} | {t.StartDate} | {t.EndDate}");
}

// Balance checks
var balanceOn6April = user.GetBalanceOn(new DateOnly(2026, 4, 6));
Console.WriteLine($"Balance on 6 April: {balanceOn6April}");

var balanceOn6May = user.GetBalanceOn(new DateOnly(2026, 5, 6));
Console.WriteLine($"Balance on 6 May: {balanceOn6May}");

var balanceOn6June = user.GetBalanceOn(new DateOnly(2026, 6, 6));
Console.WriteLine($"Balance on 6 June: {balanceOn6June}");
*/

//app.Run();