# Story intake

- Folder: `.squad/stories/tickets/assign-ticket/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-028`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Ticket Assignment
```

---

## Description

```
Source Requirements: FR-019
Feature: F02 - Ticket Management
Actor: Supervisor, Administrator (assigning); Agent (receiving the assignment)

Business goal:
As a Supervisor or Administrator, I want to assign a ticket to an Agent so that
responsibility for resolving it is clear.

User Story: As a Supervisor or Administrator, I want to assign a ticket to an
Agent, so that responsibility for resolving it is clear.

Business value: Without assignment, tickets have no clear owner - core to the
support workflow.

Priority: Must Have.

Business rules (from documentation):
- Ticket can be assigned (F02 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Supervisor or Administrator and an existing ticket and Agent
When they assign the ticket to that Agent
Then the ticket's assignee is set and the assignment is recorded

Given an attempt to assign a ticket to a user who is not an Agent
When the assignment is submitted
Then it is rejected with a validation error

Given an Agent (not Supervisor/Administrator)
When they attempt to assign a ticket through this capability
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** feeds `tickets/view-ticket-history` and `agent-dashboard/view-assigned-tickets`; related to `sla-automation/automatic-ticket-assignment` (the automated counterpart to this manual assignment).

## Extra notes (optional)

- Open question: whether an Agent may self-assign an unassigned ticket, or only Supervisor/Administrator may assign, is not specified in the documentation; scoped conservatively to Supervisor/Administrator per F00's role hierarchy implication, pending clarification.

## Technical hints (optional)

- Adds an `AssignedAgentId` (or similar) reference on `Ticket` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Automatic/rule-based assignment (see "Automatic Ticket Assignment").
