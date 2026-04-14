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

// original recurring transaction
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

var salary = user.RecurringTransactions
    .Single(t => t.Kind == RecurringTransactionKind.Salary);

try
{
    user.EndRecurringTransaction(
        salary.Id,
        new DateOnly(2026, 4, 30));
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}

// inspect results
foreach (var t in user.RecurringTransactions)
{
    Console.WriteLine($"{t.Description} | {t.Amount} | {t.StartDate} | {t.EndDate}");
}

//app.Run();