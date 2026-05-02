# AGENTS.md

## Purpose

This repository is a learning-focused finance tracking application built to practice real-world .NET backend development and, later, frontend integration. The goal is to prioritise understanding, clear boundaries, and incremental delivery over speed.

## How To Work In This Repo

- Work in small, focused steps.
- Do not make large multi-area rewrites unless explicitly asked.
- Explain reasoning before or alongside code changes when the user is asking for guidance.
- Keep responses short, direct, and technical.
- If the next step depends on a design choice, explain the tradeoff clearly before proceeding.
- Treat this repository as a teaching and building session, not just code generation.

## Architecture Expectations

- Preserve Clean Architecture boundaries.
- Domain is the source of truth for business rules and invariants.
- Application orchestrates use cases and should stay thin.
- Infrastructure contains persistence and technical wiring.
- API translates HTTP requests into Application use cases.
- Do not move business rules out of Domain unless explicitly requested.
- Do not bypass the aggregate by mutating child entities directly from API or Infrastructure.

## Domain Rules To Preserve

- `User` is the aggregate root.
- `RecurringTransaction` and `OneOffTransaction` are owned by `User`.
- Amounts are positive; direction comes from `TransactionType`.
- Salary is modelled as a recurring transaction.
- User creation must include a salary.
- Balances are calculated, not stored as a mutable running total.
- Salary history must be preserved through replacement, not overwrite.
- Only one salary may be active on a given date.
- Salary ranges must not overlap.
- Cycle calculations must use the salary active on the supplied date.

## Change Rules

- Prefer small, targeted edits.
- Do not refactor unrelated code while solving the current task.
- Do not rewrite working code for style alone.
- Keep naming and structure consistent with the existing codebase.
- Add comments only when they clarify non-obvious logic.

## Security And Safety Rules

- Do not print, expose, or copy secrets into responses, logs, tests, docs, or commits.
- Do not inspect secret-bearing files unless the task clearly requires it.
- If secret or environment configuration is needed, ask for a placeholder or example value instead of revealing a real one.
- Do not run destructive commands unless explicitly requested.

## Command Rules

- Safe read-only exploration is allowed.
- Do not run database migration, database update, deployment, cleanup, or force-push commands without explicit approval.
- Do not run broad environment-dumping commands just to gather context.
- If a command may change data outside the repo or has non-obvious side effects, pause and ask first.

## Documentation Rules

- Keep project docs accurate to the current codebase, not to outdated plans.
- Use `docs/` for project context such as architecture, domain rules, status, and roadmap.
- Update docs when code changes materially affect system understanding or workflow.

## Testing Rules

- Run relevant tests after making code changes when the environment allows it.
- If tests cannot be run, say so clearly and state why.
- Do not claim verification that did not happen.

## Response Style

- Use plain, literal, direct explanations.
- Avoid rhetorical phrasing, punchy one-liners, or repeated restatement.
- Prefer concise technical explanations that add new information sentence by sentence.
- Keep focus on what changed, why it changed, and any remaining risks or gaps.
