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

var userId = Guid.NewGuid();

var user = User.Create(
    userId,
    "Ryan",
    1000m,
    new DateOnly(2026, 1, 1),
    2000m,
    25);

Console.WriteLine("=== Initial user created ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 1: Try to add a second overlapping salary ===");

try
{
    var secondSalary = new RecurringTransaction(
        Guid.NewGuid(),
        user.Id,
        "Second Salary",
        2500m,
        TransactionType.Income,
        RecurringTransactionKind.Salary,
        new DateOnly(2026, 3, 1),
        null,
        25);

    user.AddRecurringTransaction(secondSalary);

    Console.WriteLine("Second salary added successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 1 ===");
PrintRecurringTransactions(user);

static void PrintRecurringTransactions(User user)
{
    foreach (var transaction in user.RecurringTransactions)
    {
        Console.WriteLine(
            $"Id: {transaction.Id} | " +
            $"Description: {transaction.Description} | " +
            $"Kind: {transaction.Kind} | " +
            $"Type: {transaction.Type} | " +
            $"Amount: {transaction.Amount} | " +
            $"Start: {transaction.StartDate} | " +
            $"End: {(transaction.EndDate.HasValue ? transaction.EndDate.Value.ToString() : "null")} | " +
            $"Day: {transaction.ScheduledDayOfMonth}");
    }
}

Console.WriteLine();
Console.WriteLine("=== Scenario 2: Add a recurring expense ===");

RecurringTransaction? gym = null;

try
{
    gym = new RecurringTransaction(
        Guid.NewGuid(),
        user.Id,
        "Gym",
        30m,
        TransactionType.Expense,
        RecurringTransactionKind.Expense,
        new DateOnly(2026, 2, 1),
        null,
        5);

    user.AddRecurringTransaction(gym);

    Console.WriteLine("Gym recurring expense added successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 2 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 3: End the recurring expense ===");

if (gym is not null)
{
    try
    {
        user.EndRecurringTransaction(gym.Id, new DateOnly(2026, 6, 30));
        Console.WriteLine("Gym recurring expense ended successfully.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Unexpected failure: {ex.Message}");
    }
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 3 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 4: Try to end salary directly ===");

var salary = user.RecurringTransactions.Single(t => t.Kind == RecurringTransactionKind.Salary);

try
{
    user.EndRecurringTransaction(salary.Id, new DateOnly(2026, 6, 30));
    Console.WriteLine("Salary ended successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 4 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 5: Replace salary with a new version ===");

try
{
    user.ReplaceRecurringTransaction(
        salary.Id,
        "Salary",
        2200m,
        25,
        new DateOnly(2026, 5, 1));

    Console.WriteLine("Salary replaced successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 5 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 6: Try to replace the old ended salary again ===");

var oldSalary = user.RecurringTransactions
    .Single(t => t.Kind == RecurringTransactionKind.Salary && t.EndDate.HasValue);

try
{
    user.ReplaceRecurringTransaction(
        oldSalary.Id,
        "Salary",
        2300m,
        25,
        new DateOnly(2026, 7, 1));

    Console.WriteLine("Old salary replaced successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 6 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 7: Try invalid replacement and confirm original active salary stays unchanged ===");

var activeSalary = user.RecurringTransactions
    .Single(t => t.Kind == RecurringTransactionKind.Salary && !t.EndDate.HasValue);

try
{
    user.ReplaceRecurringTransaction(
        activeSalary.Id,
        "",
        2400m,
        25,
        new DateOnly(2026, 8, 1));

    Console.WriteLine("Invalid replacement succeeded.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 7 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 8: Try to add expense with wrong type ===");

try
{
    var badExpense = new RecurringTransaction(
        Guid.NewGuid(),
        user.Id,
        "Broken Expense",
        50m,
        TransactionType.Income,
        RecurringTransactionKind.Expense,
        new DateOnly(2026, 3, 1),
        null,
        10);

    user.AddRecurringTransaction(badExpense);

    Console.WriteLine("Bad expense added successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== State after Scenario 8 ===");
PrintRecurringTransactions(user);

Console.WriteLine();
Console.WriteLine("=== Scenario 9: Try to add recurring transaction before user start date ===");

try
{
    var tooEarlyExpense = new RecurringTransaction(
        Guid.NewGuid(),
        user.Id,
        "Too Early",
        40m,
        TransactionType.Expense,
        RecurringTransactionKind.Expense,
        new DateOnly(2025, 12, 1),
        null,
        1);

    user.AddRecurringTransaction(tooEarlyExpense);

    Console.WriteLine("Too early expense added successfully.");
}
catch (Exception ex)
{
    Console.WriteLine($"Expected failure: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("=== Final state ===");
PrintRecurringTransactions(user);

//app.Run();