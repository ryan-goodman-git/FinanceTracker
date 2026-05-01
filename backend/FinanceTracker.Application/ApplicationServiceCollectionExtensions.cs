using Microsoft.Extensions.DependencyInjection;

using GetBalanceForUserOnDate = FinanceTracker.Application.Queries.GetBalanceForUserOnDate;
using GetProjectedSavingsForUser = FinanceTracker.Application.Queries.GetProjectedSavingsForUser;
using GetUserById = FinanceTracker.Application.Queries.GetUserById;
using GetOneOffTransaction = FinanceTracker.Application.Queries.GetOneOffTransactionById;
using GetRecurringTransaction = FinanceTracker.Application.Queries.GetRecurringTransactionById;

using CreateUser = FinanceTracker.Application.Commands.CreateUser;
using AddOneOffTransaction = FinanceTracker.Application.Commands.AddOneOffTransaction;
using EditOneOffTransaction = FinanceTracker.Application.Commands.EditOneOffTransaction;
using DeleteOneOffTransaction = FinanceTracker.Application.Commands.DeleteOneOffTransaction;
using AddRecurringTransaction = FinanceTracker.Application.Commands.AddRecurringTransaction;
using ReplaceRecurringTransaction = FinanceTracker.Application.Commands.ReplaceRecurringTransaction;
using EndRecurringTransaction = FinanceTracker.Application.Commands.EndRecurringTransaction;



namespace FinanceTracker.Application;

public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<GetUserById.Handler>();
        services.AddScoped<GetBalanceForUserOnDate.Handler>();
        services.AddScoped<GetProjectedSavingsForUser.Handler>();
        services.AddScoped<GetOneOffTransaction.Handler>();
        services.AddScoped<GetRecurringTransaction.Handler>();

        services.AddScoped<CreateUser.Handler>();
        services.AddScoped<AddOneOffTransaction.Handler>();
        services.AddScoped<EditOneOffTransaction.Handler>();
        services.AddScoped<DeleteOneOffTransaction.Handler>();
        services.AddScoped<AddRecurringTransaction.Handler>();
        services.AddScoped<ReplaceRecurringTransaction.Handler>();
        services.AddScoped<EndRecurringTransaction.Handler>();
        
        return services;
    }
}