# Architecture

## Layers

- Api
- Application
- Domain
- Infrastructure

## Dependency Direction

- Api -> Application, Infrastructure
- Application -> Domain
- Infrastructure -> Application, Domain
- Domain -> none

## Layer Responsibilities

### Api

- Defines HTTP contracts and minimal API endpoints
- Translates requests into Application commands and queries
- Maps results and exceptions into HTTP responses

### Application

- Defines use cases as commands and queries
- Loads aggregates through `IUserRepository`
- Calls Domain methods and persists aggregate changes
- Does not own business rules

### Domain

- Holds business rules and invariants
- Owns the `User` aggregate and transaction entities
- Calculates balances, projections, and salary-cycle behaviour

### Infrastructure

- Implements persistence and technical wiring
- Provides `EfUserRepository`
- Configures EF Core `DbContext`, entity mappings, and migrations
- Registers infrastructure services in dependency injection

## Current Wiring

- `FinanceTracker.Api/Program.cs` registers Swagger, Application services, and Infrastructure services
- `FinanceTracker.Application/ApplicationServiceCollectionExtensions.cs` registers command and query handlers
- `FinanceTracker.Infrastructure/InfrastructureServiceCollectionExtensions.cs` registers SQL Server `DbContext` and `IUserRepository`

## Architectural Rules

- Domain is the source of truth for business rules
- Application orchestrates use cases but should stay thin
- Infrastructure provides persistence and technical implementations
- Api should not contain business logic
- Infrastructure and Api should not bypass aggregate rules by mutating child entities directly
