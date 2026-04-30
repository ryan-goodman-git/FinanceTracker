using FinanceTracker.Api.Contracts.Requests.RecurringTransactions;
using FinanceTracker.Api.Contracts.Responses.RecurringTransactions;
using FinanceTracker.Api.Errors;

using AddRecurringTransaction = FinanceTracker.Application.Commands.AddRecurringTransaction;
using ReplaceRecurringTransaction = FinanceTracker.Application.Commands.ReplaceRecurringTransaction;
using EndRecurringTransaction = FinanceTracker.Application.Commands.EndRecurringTransaction;

namespace FinanceTracker.Api.Endpoints.RecurringTransactions;

public static class RecurringTransactionEndpoints
{
    public static IEndpointRouteBuilder MapRecurringTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{userId:guid}/recurring-transactions", (
            Guid userId,
            AddRecurringTransactionRequest request,
            AddRecurringTransaction.Handler handler) =>
        {
            try
            {
                var command = new AddRecurringTransaction.Command(
                    userId,
                    request.Description,
                    request.Amount,
                    request.ScheduledDayOfMonth,
                    request.StartDate);
                
                var result = handler.Handle(command);
                
                var response = new AddRecurringTransactionResponse(result.RecurringTransactionId);
                
                return Results.Created(
                    $"/users/{userId}/recurring-transactions/{response.RecurringTransactionId}",
                    response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });

        app.MapPut("/users/{userId:guid}/recurring-transactions/{recurringTransactionId:guid}", (
            Guid userId,
            Guid recurringTransactionId,
            ReplaceRecurringTransactionRequest request,
            ReplaceRecurringTransaction.Handler handler) =>
        {
            try
            {
                var command = new ReplaceRecurringTransaction.Command(
                    userId,
                    recurringTransactionId,
                    request.Description,
                    request.Amount,
                    request.ScheduledDayOfMonth,
                    request.ReplacementStartDate);
                
                var result = handler.Handle(command);
                
                var response = new ReplaceRecurringTransactionResponse(result.RecurringTransactionId);
                
                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });

        app.MapDelete("/users/{userId:guid}/recurring-transactions/{recurringTransactionId:guid}", (
            Guid userId,
            Guid recurringTransactionId,
            DateOnly endDate,
            EndRecurringTransaction.Handler handler) =>
        {
            try
            {
                var command = new EndRecurringTransaction.Command(
                    userId,
                    recurringTransactionId,
                    endDate);
                
                handler.Handle(command);
                
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));

            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });
        
        return app;
    }
}