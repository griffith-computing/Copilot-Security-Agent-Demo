# Copilot Security Agent Demo

This repository demonstrates a GitHub Copilot security-review orchestrator and
a small ASP.NET Core API that can be used as its review target.

## Security-review orchestrator

The reusable custom agent
`.github/agents/security-review.agent.md` delegates a review to four
independent subagents:

- untrusted input and injection;
- authentication and authorization;
- sensitive data and cryptography;
- dependencies and security configuration.

Each specialist agent is stored beside the orchestrator. The orchestrator
requires file-and-line evidence, removes duplicates, and produces a single
severity-ranked report.

In a Copilot Chat client that supports repository custom agents, select the
`security-review` agent. Select files first to narrow the scope, or start
without a selection to review the repository.

## CodeQL security analysis

The `codeql-security` custom agent coordinates focused reviews of injection,
access-control, and data-protection query categories through specialist agents
in `.github/agents`. It returns a consolidated report and only describes a
CodeQL query as executed when CodeQL results are available in the review
context.

The `CodeQL security analysis` GitHub Actions workflow runs CodeQL's
`security-extended` query suite against the C# project for pushes, pull
requests, and manual dispatches. CodeQL findings are uploaded to GitHub code
scanning.

The separate `CodeQL agent review` workflow runs the `codeql-security` Copilot
agent and its specialist agents on pull requests or manual dispatch. Its
read-only severity-ranked report is added to the job summary and uploaded as
the `codeql-agent-review` artifact for 14 days.

## Sample .NET application

`src/SecureTodoApi` is a minimal .NET 10 API with bounded request and in-memory
storage, input validation, centralized exception handling, HTTPS redirection,
HSTS outside development, and basic response hardening.

```bash
dotnet restore
dotnet build --no-restore
dotnet run --project tests/SecureTodoApi.SmokeTests --no-build
dotnet run --project src/SecureTodoApi
```

Use `src/SecureTodoApi/SecureTodoApi.http` to exercise the health and todo
endpoints.

## Automated build and security review

The `Build and security review` GitHub Actions workflow runs for pull requests
and can also be started manually with **Run workflow** from the Actions tab. It
restores and builds the .NET 10 solution, runs the smoke tests, and then invokes
the repository's `security-review` custom agent against the checked-out
revision.

The agent receives read-only repository access. Its severity-ranked Markdown
report is added to the job summary and retained as the `security-review`
artifact for 14 days. The workflow grants only `contents: read` and
`copilot-requests: write`; it does not grant repository write access or persist
checkout credentials.

No repository secrets are required. The organization must enable Copilot CLI
and allow its use in GitHub Actions with the built-in `GITHUB_TOKEN`. Copilot
requests are billed to the organization.

Run the deterministic validation locally before submitting changes:

```bash
dotnet restore CopilotSecurityAgentDemo.slnx
dotnet build CopilotSecurityAgentDemo.slnx --configuration Release --no-restore
dotnet run --project tests/SecureTodoApi.SmokeTests --configuration Release --no-build
```