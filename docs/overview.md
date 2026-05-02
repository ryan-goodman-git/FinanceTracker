# Overview

## Project

- Name: Finance Tracker
- Scope: full-stack finance tracking application
- Purpose: learning project for real-world backend and frontend application development

## Learning Goals

- Learn C# and .NET through a realistic application
- Learn how a frontend and backend fit together in a practical application
- Practice OOP, encapsulation, aggregates, invariants, and clean code
- Apply Clean Architecture in a practical codebase
- Build interview-ready understanding of design decisions
- Prioritise understanding over speed

## Product Goal

- Track a user's financial position over time
- Return balance on any requested date
- Calculate projected savings for the current salary cycle
- Support a very small user count (Min 2 - Max 1)

## Current Scope

- The backend already includes the core domain model, application use cases, EF Core persistence, SQL-backed infrastructure, and HTTP API endpoints for the main single-user finance flows
- Single-user backend flows are implemented end to end through Domain, Application, Infrastructure, and API layers
- Balance and projected savings queries are implemented
- One-off transaction create, read, update, and delete flows are implemented
- Recurring transaction create, read, replace, and end flows are implemented
- Salary is modelled as a recurring transaction and drives cycle calculations
- The application is intended to be a full-stack system with an Angular and TypeScript frontend consuming the backend API

## Tech Stack

- .NET Web API
- C#
- Frontend (Angular)
- xUnit for automated tests
- Clean Architecture
- EF Core
- SQL Server
- Azure-hosted database and planned Azure-hosted application deployment

## Current Persistence Setup

- Infrastructure uses EF Core with SQL Server
- Migrations exist in the Infrastructure project
- `DefaultConnection` is configured through app settings
- The database is hosted in Azure
- The application is expected to be hosted in Azure
