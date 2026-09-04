# Story intake

- Folder: `.squad/stories/tickets/track-ticket/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-026`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Track Ticket
```

---

## Description

```
Source Requirements: FR-016
Feature: F02 - Ticket Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to view a ticket's current details and progress so that I can see
where it stands.

User Story: As staff, I want to view a ticket's current details and progress, so
that I can see where it stands.

Business value: Without this, a created ticket is a black box - core to any
support workflow.

Priority: Must Have.

Business rules (from documentation):
- Ticket can be created and tracked (F02 acceptance criteria) - this story covers
  "tracked" as viewing a ticket's current state.
```

---

## Acceptance criteria

```
Given authenticated staff and an existing ticket id
When they request that ticket's details
Then the ticket's current subject/description, status, priority, category, and
  assignee (as set by their respective stories) are returned

Given authenticated staff
When they request a list of tickets
Then a list of tickets is returned

Given a ticket id that does not exist
When it is requested
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
- **Depends on code areas or other stories:** displays data set by `tickets/set-ticket-category-and-priority`, `tickets/assign-ticket`, `tickets/change-ticket-status`, `tickets/escalate-ticket` as those ship.

## Extra notes (optional)

- Filtering/search of the ticket list is not specified in the documentation beyond "tracked"; a basic list is the minimum viable scope.

## Technical hints (optional)

- Uses the generic repository's read operations already established in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- The change-log/timeline view (see "View Ticket History").
