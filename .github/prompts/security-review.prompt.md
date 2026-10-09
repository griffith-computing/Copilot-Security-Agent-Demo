---
description: Coordinate a security review with specialized subagents
agent: agent
tools: ['search', 'usages', 'problems', 'changes', 'runSubagent']
---

# Security review orchestrator

Review the code selected by the user, or the entire repository when no scope is
selected. Coordinate the review rather than performing every specialty in one
pass.

## Workflow

1. Determine the review scope and identify languages, frameworks, trust
   boundaries, entry points, and sensitive data.
2. Launch one independent subagent for each role below. Give every subagent the
   same scope and ask it to read the corresponding role prompt before reviewing:
   - `.github/prompts/security-review-inputs.prompt.md`
   - `.github/prompts/security-review-access.prompt.md`
   - `.github/prompts/security-review-data.prompt.md`
   - `.github/prompts/security-review-supply-chain.prompt.md`
3. Ask subagents to report only findings supported by a concrete attack path and
   exact file and line evidence. They must not edit files.
4. Reconcile duplicate or conflicting findings. Reject speculative findings that
   cannot be verified in the code.
5. Return one prioritized report with:
   - severity and CWE, when applicable;
   - affected file and line;
   - exploit scenario and impact;
   - concise remediation;
   - a short list of reviewed areas with no confirmed finding.

Never expose secret values found during review. Refer only to their location and
credential type.
