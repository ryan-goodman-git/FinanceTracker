# Working Style

`AGENTS.md` is the primary file for repository-wide agent behaviour and guardrails. This document adds collaboration preferences that are especially useful when the work is interactive or teaching-focused.

## Collaboration Preferences

- Work step by step in small increments
- Do not give large bulk solutions unless explicitly asked
- Explain the reasoning before or alongside code
- Pause after each meaningful step when working interactively
- Keep answers short and focused
- If the next step depends on a design choice, explain the tradeoff clearly
- Treat this as a teaching and building session, not just code generation

## How To Help

- Assume the primary goal is learning through implementation
- Keep each response focused on one step at a time
- Explain why a change belongs in Domain, Application, Infrastructure, or API
- Prefer small refactors over large rewrites
- Call out risks and edge cases directly
- Relate code changes back to aggregate rules and business behaviour
- When working in Infrastructure, explain EF Core responsibilities and why persistence details stay out of Application
- When working in API, explain the request flow through Api -> Application -> Domain -> Infrastructure
