# Domain Model

## Aggregate

- Root: `User`
- Children: `RecurringTransaction`, `OneOffTransaction`

## Core Entities

- `User`: aggregate root that owns transaction collections and all mutation logic
- `RecurringTransaction`: monthly repeating income or expense with a scheduled day and date range
- `OneOffTransaction`: single dated income or expense

## Enums

- `TransactionType = Income | Expense`
- `RecurringTransactionKind = Salary | Expense`

## Current Design Decisions

- Amount is always positive; transaction direction comes from `TransactionType`
- Salary is modelled as a `RecurringTransaction`
- `User` is the aggregate root and owns mutation logic
- Collections are stored as private `List<T>` fields and exposed as `IReadOnlyCollection<T>`
- `User` must be created through a factory method
- User creation must include a salary transaction
- Balances are calculated, not stored as a mutable running total
- Balance = initial balance + valid one-off transactions + valid recurring occurrences up to a target date
- Recurring transactions have `StartDate` and nullable `EndDate`
- Null `EndDate` means the recurring transaction is active until replaced or ended
- Salary changes are modelled as timeline versions rather than overwriting history
- Only one salary may be active on a given date
- Salary date ranges must not overlap
- Cycle calculations use the salary active on the supplied date
- Cycle end date is the day before the next salary date
- Scheduled salary days clamp correctly for short months
- Next cycle calculation is based on the original scheduled day, not a previously clamped date
- Recurring replacement takes an explicit replacement start date
- Replacement validates fully before mutating the existing transaction
- Salary cannot be ended directly through `EndRecurringTransaction`
- One-off transactions can currently be edited for description and amount only
- One-off transactions can currently be deleted through the `User` aggregate
- `ReplaceRecurringTransaction` returns the created replacement transaction

## Business Rules

### User Rules

- User id cannot be empty
- User name is required
- User must be created with a salary transaction
- The product is intended to support only a very small number of users (min 1 - max 2)

### Recurring Transaction Rules

- Recurring transaction must belong to the user
- Recurring transaction cannot start before `User.StartDate`
- Salary must always be `Income`
- Recurring expense must always be `Expense`
- Salary timeline versions must not overlap
- Only one salary may be active on a given date
- Recurring occurrences count only when the occurrence date is within the transaction's active range
- Replacing a recurring transaction ends the old version the day before the replacement start date and creates a new version from the replacement start date
- Replacement start date must be after the existing transaction start date
- A recurring transaction cannot be replaced after it has already ended
- Salary transactions cannot be ended directly through `EndRecurringTransaction`

### One-Off Transaction Rules

- One-off transaction must belong to the user
- One-off transaction cannot be before `User.StartDate`
- One-off transaction description is required
- One-off transaction amount must be greater than zero

### Cycle Rules

- Cycle starts on salary day
- Cycle ends the day before the next salary day
- Projected savings equals balance on the current cycle end date
- Projection uses the salary active on the supplied date

## Current Domain Boundaries

- Domain owns all financial rules and date logic
- Application may create entity instances and call aggregate methods, but it should not duplicate business rules
- Infrastructure persists aggregate state and should not change behaviour
