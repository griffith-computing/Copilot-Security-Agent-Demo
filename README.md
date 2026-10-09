# Copilot Security Agent Demo

This repository demonstrates a GitHub Copilot security-review orchestrator and
a small ASP.NET Core API that can be used as its review target.

## Security-review orchestrator

The reusable prompt
`.github/prompts/security-review.prompt.md` delegates a review to four
independent subagents:

- untrusted input and injection;
- authentication and authorization;
- sensitive data and cryptography;
- dependencies and security configuration.

Each specialist prompt is stored beside the orchestrator. The orchestrator
requires file-and-line evidence, removes duplicates, and produces a single
severity-ranked report.

In a Copilot Chat client that supports repository prompt files, run
`/security-review` from the repository root. Select files first to narrow the
scope, or run it without a selection to review the repository.

## Sample .NET application

`src/SecureTodoApi` is a minimal .NET 10 API with bounded request and in-memory
storage, input validation, centralized exception handling, HTTPS redirection,
HSTS outside development, and basic response hardening.

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project src/SecureTodoApi
```

Use `src/SecureTodoApi/SecureTodoApi.http` to exercise the health and todo
endpoints.