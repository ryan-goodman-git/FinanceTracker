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

var user = User.Create(Guid.NewGuid(), "Ryan", 1000m, new DateOnly(2025, 1, 1), 2000m, 25);
Console.WriteLine($"User created{user.Name}");
Console.WriteLine($"Recurring transactions count {user.RecurringTransactions.Count}");

var secondSalary = new RecurringTransaction(
    Guid.NewGuid(),
    user.Id,
    "Second Salary",
    500m,
    TransactionType.Income,
    RecurringTransactionKind.Salary,
    1);

user.AddRecurringTransaction(secondSalary);

app.Run();
