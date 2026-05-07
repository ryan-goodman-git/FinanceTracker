# Roadmap

## Current Priority

- Stabilise the backend in the areas needed to support the new frontend work
- Improve exception handling and HTTP response mapping so API behaviour is correct and consistent
- Add integration test coverage for the API and Infrastructure layers

## Next Work

- Add startup frontend logic to route to `Users` when there are 0 users and `Overview` when there are 1 or 2 users
- Design backend read model and endpoint for Overview page data
- Implement clearer exception classification so missing resources and invalid requests are handled differently
- Update API endpoints to return more accurate HTTP status codes for not-found, invalid input, and business-rule failures
- Add API integration tests for endpoint behaviour, response codes, and error responses
- Add Infrastructure integration tests for EF Core persistence, aggregate loading, and update behaviour
- Remove temporary debugging code from persistence
- Add frontend error display for failed create-user submissions

## API Expansion

- Add Overview-focused read endpoint when the Overview page data shape is clearer
- Keep salary represented as a recurring transaction in backend responses
- Revisit combined-user queries later once the frontend data needs are clearer

## Domain and Policy Follow-Ups

- Enforce the maximum of two users in the system
- Restrict one-off transactions so they can only be added for today or future dates
- Restrict one-off transactions so they can only be edited or deleted for today or future dates
- Keep one-off transaction editing limited to description and amount only
- Add weekend shifting rules to salary-cycle calculations

## Do Not Do Yet

- Do not add combined-user queries until the frontend data needs are clearer
- Do not expand the API surface beyond current priorities until exception handling and integration testing are in place
