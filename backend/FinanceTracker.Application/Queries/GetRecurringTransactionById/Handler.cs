using FinanceTracker.Application.Interfaces;

namespace FinanceTracker.Application.Queries.GetRecurringTransactionById;

public sealed class Handler
{
    private readonly IUserRepository _repository;

    public Handler(IUserRepository repository)
    {
        _repository = repository;
    }

    public Result Handle(Query query)
    {
        var user = _repository.GetById(query.UserId);

        if (user is null)
            throw new InvalidOperationException("User was not found.");

        var transaction = user.RecurringTransactions.SingleOrDefault(t => t.Id == query.RecurringTransactionId);

        if (transaction is null)
            throw new InvalidOperationException("Recurring transaction was not found.");

        return new Result(
            transaction.Id,
            transaction.UserId,
            transaction.Description,
            transaction.Amount,
            transaction.Type,
            transaction.Kind,
            transaction.ScheduledDayOfMonth,
            transaction.StartDate,
            transaction.EndDate);
    }
}