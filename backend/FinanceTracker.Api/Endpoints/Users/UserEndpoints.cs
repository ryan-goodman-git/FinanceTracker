using FinanceTracker.Api.Contracts.Requests.Users;
using CreateUser = FinanceTracker.Application.Commands.CreateUser;
using GetBalanceForUserOnDate = FinanceTracker.Application.Queries.GetBalanceForUserOnDate;
using GetProjectedSavingsForUser = FinanceTracker.Application.Queries.GetProjectedSavingsForUser;

namespace FinanceTracker.Api.Endpoints.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/users", (CreateUserRequest request, CreateUser.Handler handler) =>
        {
            var command = new CreateUser.Command(
                request.Name,
                request.InitialBalance,
                request.StartDate,
                request.SalaryAmount,
                request.SalaryDayOfMonth);

            var result = handler.Handle(command);

            return Results.Ok(result);
        });

        app.MapGet("/users/{userId:guid}/balance", (
            Guid userId,
            DateOnly targetDate,
            GetBalanceForUserOnDate.Handler handler) =>
        {
            var query = new GetBalanceForUserOnDate.Query(userId, targetDate);
            var result = handler.Handle(query);

            return Results.Ok(result);
        });
        
        app.MapGet("/users/{userId:guid}/projected-savings", (
            Guid userId,
            DateOnly today,
            GetProjectedSavingsForUser.Handler handler) =>
        {
            var query = new GetProjectedSavingsForUser.Query(userId, today);
            var result = handler.Handle(query);

            return Results.Ok(result);
        });

        return app;
    }
}