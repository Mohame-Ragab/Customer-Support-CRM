# Story intake

- Folder: `.squad/stories/tickets/view-ticket-history/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-031`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Ticket History
```

---

## Description

```
Source Requirements: FR-022
Feature: F02 - Ticket Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to see a ticket's full history (status changes, assignment
changes, category/priority changes, escalations) so that I can understand what
happened to it over time.

User Story: As staff, I want to see a ticket's full history (status changes,
assignment changes, category/priority changes, escalations), so that I can
understand what happened to it over time.

Business value: Valuable audit/context trail; not required to resolve a single
ticket.

Priority: Should Have.

Business rules (from documentation):
- Ticket history is available (F02 acceptance criteria).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing ticket
When they view its history
Then a chronological timeline of the ticket's changes (creation, category/priority
  changes, assignment changes, status changes, escalations) is returned, each entry
  attributed to who made the change and when

Given a newly created ticket with no changes yet
When its history is viewed
Then only the creation event is present (not an error)

Given a ticket id that does not exist
When its history is requested
Then a not-found error is returned
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`; populated by `tickets/set-ticket-category-and-priority`, `tickets/assign-ticket`, `tickets/change-ticket-status`, `tickets/escalate-ticket`.
- **Depends on code areas or other stories:** see above.

## Extra notes (optional)

- None beyond the dependency note above.

## Technical hints (optional)

- A `TicketHistoryEntry` (or similar) record appended by each of the ticket-lifecycle stories above, in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- None beyond what's already covered by the dependent stories.
