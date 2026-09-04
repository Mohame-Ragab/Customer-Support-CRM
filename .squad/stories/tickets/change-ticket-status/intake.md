# Story intake

- Folder: `.squad/stories/tickets/change-ticket-status/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-029`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Ticket Status
```

---

## Description

```
Source Requirements: FR-020
Feature: F02 - Ticket Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to change a ticket's status so that its lifecycle (new, in
progress, resolved, etc.) is reflected accurately.

User Story: As staff, I want to change a ticket's status, so that its lifecycle
(new, in progress, resolved, etc.) is reflected accurately.

Business value: Core to knowing whether a ticket is open, in progress, or
resolved.

Priority: Must Have.

Business rules (from documentation):
- Ticket ... status changed (F02 acceptance criteria).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing ticket
When they change its status to a valid status value
Then the ticket's status is updated and the change is recorded

Given an attempt to set an invalid/unknown status value
When it is submitted
Then it is rejected with a validation error

Given a ticket id that does not exist
When a status change is attempted
Then a not-found error is returned
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** feeds `tickets/view-ticket-history` and `customer-portal/track-ticket-requests`.

## Extra notes (optional)

- Open question / assumption: the documentation does not enumerate the specific status values or the allowed transitions between them. A minimal, reasonable status set (e.g. New, In Progress, Resolved, Closed) is assumed as a starting point pending product confirmation; no specific transition rules (e.g. "cannot reopen a Closed ticket") are asserted here since none are documented.

## Technical hints (optional)

- `TicketStatus` enum on `Ticket` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Escalation as a distinct concept (see "Escalate Ticket") - escalation may or may not itself be modeled as a status; kept as a separate story since it has its own FR (FR-021).
