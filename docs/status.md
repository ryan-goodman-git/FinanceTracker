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
- Frontend components created so far:
  - `UsersEmptyState`
  - `CreateUserModal`
  - `UserCard`
  - `AddUserSlot`
- Local create-user flow works in the frontend:
  - open modal
  - capture form fields
  - submit data to parent page state
  - render 0-user, 1-user, and 2-user UI states
- Current frontend user state is temporary and local only; it is not connected to the backend yet

## Testing Status

- Domain tests exist for user creation, balances, projections, one-off transaction behaviour, recurring transaction behaviour, salary overlap rules, replacement behaviour, and short-month cycle logic
- Application tests exist for the implemented command and query handlers
- Fake repository support exists for Application tests

## Known Gaps

- No integration tests for Infrastructure or API
- The maximum limit of two users in the system is not enforced yet
- `GetCombinedBalanceOnDate` is not implemented
- `GetCombinedProjectedSavings` is not implemented
- One-off transaction editing is currently limited to description and amount only
- One-off transactions are not yet restricted so they can only be added for today or future dates
- One-off transactions are not yet restricted so they can only be edited or deleted for today or future dates
- Weekend shifting logic is not implemented yet
- Frontend Users page is not connected to backend data or backend mutations yet
- Backend endpoints required for the Users page UI, such as `GET /users` and user-focused transaction data loading, do not exist yet

## Notes

- This file should track the current codebase, not the original plan
- Update this file when implemented behaviour, endpoints, or test coverage changes
