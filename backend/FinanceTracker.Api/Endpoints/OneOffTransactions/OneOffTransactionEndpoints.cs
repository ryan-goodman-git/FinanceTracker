using FinanceTracker.Api.Contracts.Requests.OneOffTransactions;
using FinanceTracker.Api.Contracts.Responses.OneOffTransactions;
using FinanceTracker.Api.Errors;

using AddOneOffTransaction = FinanceTracker.Application.Commands.AddOneOffTransaction;
using EditOneOffTransaction = FinanceTracker.Application.Commands.EditOneOffTransaction;
using DeleteOneOffTransaction = FinanceTracker.Application.Commands.DeleteOneOffTransaction;
using GetOneOffTransactionById = FinanceTracker.Application.Queries.GetOneOffTransactionById;

namespace FinanceTracker.Api.Endpoints.OneOffTransactions;

public static class OneOffTransactionEndpoints
{
    public static IEndpointRouteBuilder MapOneOffTransactionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users/{userId:guid}/one-off-transactions", (
            Guid userId,
            AddOneOffTransactionRequest oneOffTransactionRequest, 
            AddOneOffTransaction.Handler handler) =>
        {
            try
            {
                var command = new AddOneOffTransaction.Command(
                    userId,
                    oneOffTransactionRequest.Description,
                    oneOffTransactionRequest.Amount,
                    oneOffTransactionRequest.Type,
                    oneOffTransactionRequest.Date);
                
                var result = handler.Handle(command);

                var response = new AddOneOffTransactionResponse(result.OneOffTransactionId);
                
                return Results.Created(
                    $"/users/{userId}/one-off-transactions/{response.OneOffTransactionId}",
                    response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });
        
        app.MapGet("/users/{userId:guid}/one-off-transactions/{transactionId:guid}", (
            Guid userId,
            Guid transactionId,
            GetOneOffTransactionById.Handler handler) =>
        {
            try
            {
                var query = new GetOneOffTransactionById.Query(userId, transactionId);
                var result = handler.Handle(query);

                var response = new GetOneOffTransactionResponse(
                    result.OneOffTransactionId,
                    result.UserId,
                    result.Description,
                    result.Amount,
                    result.Type,
                    result.Date);

                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new ApiError(ex.Message));
            }
        });

        app.MapPut("/users/{userId:guid}/one-off-transactions/{transactionId:guid}", (
            Guid userId,
            Guid transactionId,
            EditOneOffTransactionRequest editOneOffTransactionRequest,
            EditOneOffTransaction.Handler handler) =>
        {
            try
            {
                var command = new EditOneOffTransaction.Command(
                    userId,
                    transactionId,
                    editOneOffTransactionRequest.Description,
                    editOneOffTransactionRequest.Amount);
                
                handler.Handle(command);
                
                return Results.NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
            catch(ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });

        app.MapDelete("/users/{userId:guid}/one-off-transactions/{transactionId:guid}", (
            Guid userId,
            Guid transactionId,
            DeleteOneOffTransaction.Handler handler) =>
        {
            try
            {
                var command = new DeleteOneOffTransaction.Command(
                    userId,
                    transactionId);
                
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