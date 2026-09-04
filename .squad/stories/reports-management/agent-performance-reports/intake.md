# Story intake

- Folder: `.squad/stories/reports-management/agent-performance-reports/intake.md`

---

## Feature

- **Feature name (display):** F09 — Reports & Management
- **Feature slug (folder under `plans/`):** `reports-management`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-054`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F09, reports-management`

---

## Title

```
Agent Performance
```

---

## Description

```
Source Requirements: FR-048
Feature: F09 - Reports & Management
Actor: Supervisor, Administrator

Business goal:
As a Supervisor/Administrator, I want to see per-agent performance (e.g. tickets
handled, resolved, average handling time) so that I can assess team workload and
performance.

User Story: As a Supervisor/Administrator, I want to see per-agent performance
(e.g. tickets handled, resolved, average handling time), so that I can assess
team workload and performance.

Business value: Explicitly named in F09's purpose.

Priority: Should Have.

Business rules (from documentation):
- Agent performance is reportable (F09 acceptance criteria).
```

---

## Acceptance criteria

```
Given tickets have been assigned to and resolved by agents
When an authorized user requests agent performance for a period
Then per-agent metrics (e.g. tickets assigned, resolved, average time to
  resolution) are returned

Given an Agent requests to view their own performance report
When it is requested
Then this is an open question, not a settled acceptance criterion: the
  documentation does not specify whether agents can see their own metrics (see
  Extra notes) - implementation should confirm before restricting or allowing this

Given a Customer, or an Agent attempting to view another agent's report
When they attempt to access agent performance reports
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/assign-ticket`, `tickets/change-ticket-status`.
- **Depends on code areas or other stories:** feeds `reports-management/management-dashboards`.

## Extra notes (optional)

- Open question: whether an Agent can view their own performance metrics (self-service) is not specified in the documentation; scoped conservatively to Supervisor/Administrator only pending clarification.
- The exact metrics beyond "performance" are not enumerated in the documentation; a reasonable minimal set (tickets assigned/resolved, average resolution time) is used pending confirmation.

## Technical hints (optional)

- Aggregation over `Ticket` grouped by `AssignedAgentId` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Ticket volume/status and SLA-specific reports (separate stories).
