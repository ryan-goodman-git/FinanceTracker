# Current Status

## Implemented Layers

- Domain
- Application
- Infrastructure
- Api

## Domain Status

- `User`, `RecurringTransaction`, and `OneOffTransaction` are implemented
- `User.Create` creates the aggregate and initial salary transaction
- `AddRecurringTransaction` validates ownership, start date, type-kind rules, and salary overlap rules
- `ReplaceRecurringTransaction` preserves history and returns the new transaction
- `EndRecurringTransaction` blocks direct salary ending
- `AddOneOffTransaction`, `EditOneOffTransaction`, and `DeleteOneOffTransaction` are implemented on the aggregate
- `GetBalanceOn`, `GetProjectedSavingsForCurrentCycle`, and `GetCurrentCycleEndDate` are implemented
- Recurring occurrence counting respects `StartDate`, `EndDate`, and short-month date clamping

## Application Status

- `IUserRepository` exists in the Application layer
- Query handlers implemented:
  - `GetUsers`
  - `GetUserById`
  - `GetBalanceForUserOnDate`
  - `GetProjectedSavingsForUser`
  - `GetOneOffTransactionById`
  - `GetRecurringTransactionById`
- Command handlers implemented:
  - `CreateUser`
  - `AddOneOffTransaction`
  - `EditOneOffTransaction`
  - `DeleteOneOffTransaction`
  - `AddRecurringTransaction`
  - `ReplaceRecurringTransaction`
  - `EndRecurringTransaction`

## Infrastructure Status

- `FinanceTrackerDbContext` exists
- EF Core entity configurations exist for `User`, `RecurringTransaction`, and `OneOffTransaction`
- `EfUserRepository` implements `GetById`, `Add`, and `Update`
- SQL Server is configured through `AddInfrastructure`
- Migrations already exist in the Infrastructure project

## API Status

- Minimal API setup exists in `FinanceTracker.Api`
- Swagger is enabled in development
- Endpoints implemented:
  - `GET /users`
  - `POST /users`
  - `GET /users/{userId}`
  - `GET /users/{userId}/balance`
  - `GET /users/{userId}/projected-savings`
  - `POST /users/{userId}/one-off-transactions`
  - `GET /users/{userId}/one-off-transactions/{transactionId}`
  - `PUT /users/{userId}/one-off-transactions/{transactionId}`
  - `DELETE /users/{userId}/one-off-transactions/{transactionId}`
  - `POST /users/{userId}/recurring-transactions`
  - `GET /users/{userId}/recurring-transactions/{recurringTransactionId}`
  - `PUT /users/{userId}/recurring-transactions/{recurringTransactionId}`
  - `DELETE /users/{userId}/recurring-transactions/{recurringTransactionId}`

## Frontend Status

- Angular frontend is scaffolded in the `frontend` folder
- App routing exists for:
  - `/overview`
  - `/users`
  - `/users/:userId`
- Users page shell exists with top navigation and route-based page rendering
- Root route currently redirects to `/users`
- Frontend components created so far:
  - `UsersEmptyState`
  - `CreateUserModal`
  - `UserCard`
  - `AddUserSlot`
- Users page now loads user summary data from the backend `GET /users` endpoint
- Users page loads each user's recurring transactions from `GET /users/{userId}/recurring-transactions`
- Users page state uses an Angular signal so the page updates when API data loads
- Users page uses a feature-level data service to combine user summaries with recurring transaction data
- Create-user submit posts to the backend `POST /users` endpoint, then reloads Users page data
- User cards currently show:
  - name
  - salary amount and scheduled day
  - recurring bill descriptions and amounts
- Overview page component exists but has no backend-driven data yet
- User details route and page component exist but are not built out yet


## Testing Status

- Domain tests exist for user creation, balances, projections, one-off transaction behaviour, recurring transaction behaviour, salary overlap rules, replacement behaviour, and short-month cycle logic
- Application tests exist for the implemented command and query handlers
- Fake repository support exists for Application tests

## Known Gaps

- No integration tests for Infrastructure or API
- The maximum limit of two users in the system is not enforced yet
- `GetCombinedBalanceOnDate` is not implemented
- `GetCombinedProjectedSavings` is not implemented
- One-off transactions are not yet restricted so they can only be added for today or future dates
- One-off transactions are not yet restricted so they can only be edited or deleted for today or future dates
- Weekend shifting logic is not implemented yet
- Frontend create-user failures leave the modal open but do not show an error message yet
- Overview page is not connected to backend data yet
- Backend read model for Overview page data does not exist yet
- `EfUserRepository.Update` still writes change-tracker state to the console

## Notes

- This file should track the current codebase, not the original plan
- Update this file when implemented behaviour, endpoints, or test coverage changes
