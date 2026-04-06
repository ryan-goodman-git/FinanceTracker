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

// var user = User.Create(Guid.NewGuid(), "Ryan", 1000m, new DateOnly(2025, 1, 1), 2000m, 25);
// Console.WriteLine($"User created{user.Name}");
// Console.WriteLine($"Recurring transactions count {user.RecurringTransactions.Count}");
//
// var secondSalary = new RecurringTransaction(
//     Guid.NewGuid(),
//     user.Id,
//     "Second Salary",
//     500m,
//     TransactionType.Income,
//     RecurringTransactionKind.Salary,
//     1);
//
// user.AddRecurringTransaction(secondSalary);

// var user = User.Create(
//     Guid.NewGuid(),
//     "Ryan",
//     1000m,
//     new DateOnly(2025, 1, 1),
//     2000m,
//     1);
//
// var groceries = new OneOffTransaction(
//     Guid.NewGuid(),
//     user.Id,
//     "Groceries",
//     100m,
//     TransactionType.Expense,
//     new DateOnly(2025, 1, 20));
//
// user.AddOneOffTransaction(groceries);
//
// var balance = user.GetBalanceOn(new DateOnly(2025, 1, 15));
//
// Console.WriteLine($"Balance on 15/01/2025: {balance}");

// var user = User.Create(
//     Guid.NewGuid(),
//     "Ryan",
//     1000m,
//     new DateOnly(2025, 1, 1),
//     2000m,
//     1);
//
// var balance = user.GetBalanceOn(new DateOnly(2025, 3, 15));
//
// Console.WriteLine($"Balance on 15/03/2025: {balance}");

// var user = User.Create(
//     Guid.NewGuid(),
//     "Ryan",
//     1000m,
//     new DateOnly(2025, 1, 1),
//     2000m,
//     15); // salary on 15th
//
// var balance = user.GetBalanceOn(new DateOnly(2025, 3, 10)); // BEFORE 15th March
//
// Console.WriteLine($"Balance on 10/03/2025: {balance}");

// var user = User.Create(
//     Guid.NewGuid(),
//     "Ryan",
//     1000m,
//     new DateOnly(2025, 1, 1),
//     2000m,
//     31); // salary on 31st
//
// var balance = user.GetBalanceOn(new DateOnly(2025, 2, 28));
//
// Console.WriteLine($"Balance on 28/02/2025: {balance}");

var user = User.Create(
    Guid.NewGuid(),
    "Ryan",
    1000m,
    new DateOnly(2025, 1, 1),
    2000m,
    1); // salary on 1st

// Add recurring expense (rent)
var rent = new RecurringTransaction(
    Guid.NewGuid(),
    user.Id,
    "Rent",
    500m,
    TransactionType.Expense,
    RecurringTransactionKind.Expense,
    5); // 5th of each month

user.AddRecurringTransaction(rent);

// Add one-off transactions
user.AddOneOffTransaction(new OneOffTransaction(
    Guid.NewGuid(),
    user.Id,
    "Groceries",
    100m,
    TransactionType.Expense,
    new DateOnly(2025, 1, 10)));

user.AddOneOffTransaction(new OneOffTransaction(
    Guid.NewGuid(),
    user.Id,
    "Bonus",
    300m,
    TransactionType.Income,
    new DateOnly(2025, 2, 15)));

var balance = user.GetBalanceOn(new DateOnly(2025, 2, 20));

Console.WriteLine($"Balance on 20/02/2025: {balance}");

//app.Run();
