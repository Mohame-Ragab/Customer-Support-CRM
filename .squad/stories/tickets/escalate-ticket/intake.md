# Story intake

- Folder: `.squad/stories/tickets/escalate-ticket/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-030`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Ticket Escalation
```

---

## Description

```
Source Requirements: FR-021
Feature: F02 - Ticket Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to manually escalate a ticket so that it gets attention from a
more senior role (e.g. a Supervisor) when I cannot resolve it myself.

User Story: As staff, I want to manually escalate a ticket, so that it gets
attention from a more senior role when I cannot resolve it myself.

Business value: Important safety-valve capability, not required for baseline
ticket handling.

Priority: Should Have.

Business rules (from documentation):
- Ticket ... escalated (F02 acceptance criteria).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing ticket
When they escalate the ticket (optionally with a reason)
Then the ticket is marked escalated and the escalation is recorded

Given a ticket id that does not exist
When an escalation is attempted
Then a not-found error is returned

Given a ticket that is already escalated
When an escalation is attempted again
Then the system handles it gracefully (documentation does not specify whether this
  is a no-op or an error; implement as idempotent unless clarified)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** feeds `tickets/view-ticket-history`; related to `sla-automation/automatic-escalation-rules` (the automated counterpart to this manual escalation).

## Extra notes (optional)

- Open question: who is notified/who an escalated ticket routes to (e.g. a specific Supervisor, a queue) is not specified in the documentation.

## Technical hints (optional)

- `IsEscalated`/`EscalatedAt` (or similar) on `Ticket` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Automated, rule-based escalation (see "Automatic Escalation Rules").
