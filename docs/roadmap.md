# Roadmap

## Current Priority

- Stabilise the backend before shifting focus to frontend work
- Improve exception handling and HTTP response mapping so API behaviour is correct and consistent
- Add integration test coverage for the API and Infrastructure layers

## Next Work

- Implement clearer exception classification so missing resources and invalid requests are handled differently
- Update API endpoints to return more accurate HTTP status codes for not-found, invalid input, and business-rule failures
- Add API integration tests for endpoint behaviour, response codes, and error responses
- Add Infrastructure integration tests for EF Core persistence, aggregate loading, and update behaviour
- Remove temporary debugging code from persistence

## API Expansion

- Add a `GET /users` endpoint if the frontend needs to display multiple users
- Add user-focused transaction listing endpoints if the frontend needs to display transaction history or dashboard views
- Revisit combined-user queries later once the frontend data needs are clearer

## Domain and Policy Follow-Ups

- Enforce the maximum of two users in the system
- Restrict one-off transactions so they can only be added for today or future dates
- Restrict one-off transactions so they can only be edited or deleted for today or future dates
- Keep one-off transaction editing limited to description and amount only
- Add weekend shifting rules to salary-cycle calculations

## Do Not Do Yet

- Do not move on to frontend work until the current backend stabilisation work is complete
- Do not add combined-user queries until the frontend data needs are clearer
- Do not expand the API surface beyond current priorities until exception handling and integration testing are in place