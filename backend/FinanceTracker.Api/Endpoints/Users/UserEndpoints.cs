using FinanceTracker.Api.Contracts.Requests.Users;
using FinanceTracker.Api.Contracts.Responses.Users;
using FinanceTracker.Api.Errors;
using CreateUser = FinanceTracker.Application.Commands.CreateUser;
using GetUserById = FinanceTracker.Application.Queries.GetUserById;
using GetBalanceForUserOnDate = FinanceTracker.Application.Queries.GetBalanceForUserOnDate;
using GetProjectedSavingsForUser = FinanceTracker.Application.Queries.GetProjectedSavingsForUser;

namespace FinanceTracker.Api.Endpoints.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", (CreateUserRequest request, CreateUser.Handler handler) =>
        {
            try
            {
                var command = new CreateUser.Command(
                    request.Name,
                    request.InitialBalance,
                    request.StartDate,
                    request.SalaryAmount,
                    request.SalaryDayOfMonth);

                var result = handler.Handle(command);

                var response = new CreateUserResponse(result.UserId);

                return Results.Created($"/users/{response.UserId}", response);
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
        
        app.MapGet("/users/{userId:guid}", (
            Guid userId,
            GetUserById.Handler handler) =>
        {
            try
            {
                var query = new GetUserById.Query(userId);
                var result = handler.Handle(query);

                var response = new GetUserResponse(
                    result.UserId,
                    result.Name,
                    result.InitialBalance,
                    result.StartDate);

                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new ApiError(ex.Message));
            }
        });

        app.MapGet("/users/{userId:guid}/balance", (
            Guid userId,
            DateOnly targetDate,
            GetBalanceForUserOnDate.Handler handler) =>
        {
            try
            {
                var query = new GetBalanceForUserOnDate.Query(userId, targetDate);
                var result = handler.Handle(query);

                var response = new GetBalanceResponse(
                    userId,
                    targetDate,
                    result.Balance);

                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new ApiError(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });
        
        app.MapGet("/users/{userId:guid}/projected-savings", (
            Guid userId,
            DateOnly today,
            GetProjectedSavingsForUser.Handler handler) =>
        {
            try
            {
                var query = new GetProjectedSavingsForUser.Query(userId, today);
                var result = handler.Handle(query);

                var response = new GetProjectedSavingsResponse(
                    userId,
                    today,
                    result.CycleEndDate,
                    result.ProjectedSavings);

                return Results.Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return Results.NotFound(new ApiError(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new ApiError(ex.Message));
            }
        });
        
        return app;
    }
}