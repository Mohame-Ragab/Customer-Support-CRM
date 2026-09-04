# Story intake

- Folder: `.squad/stories/tickets/set-ticket-category-and-priority/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-027`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Ticket Categories / Ticket Priorities
```

---

## Description

```
Source Requirements: FR-017, FR-018
Feature: F02 - Ticket Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to categorize a ticket and set its priority so that it can be
triaged and routed appropriately.

User Story: As staff, I want to categorize a ticket and set its priority, so that
it can be triaged and routed appropriately.

Business value: Improves triage quality; a ticket can still be worked without it.

Priority: Should Have.

Business rules (from documentation):
- Category and priority can be set (F02 acceptance criteria - a single sentence
  covering both FR-017 and FR-018, which is why they are combined into one story
  here rather than duplicated across two).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing ticket
When they set the ticket's category and/or priority to a valid value
Then the ticket is updated with the new category/priority

Given an invalid category or priority value is submitted
When the update is attempted
Then it is rejected with a validation error

Given a ticket id that does not exist
When a category/priority update is attempted
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
- **Depends on code areas or other stories:** feeds `tickets/view-ticket-history` (a category/priority change is a history event); category/priority may inform `sla-automation/automatic-ticket-assignment` and `sla-automation/define-sla-response-and-resolution-targets` once those exist.

## Extra notes (optional)

- Open question / assumption: the documentation does not specify the concrete list of categories or priority levels, nor whether administrators can configure that list (vs. a fixed, seeded set). A fixed, seeded set (e.g. Low/Medium/High/Urgent for priority) is assumed as the minimum viable default pending product clarification; making the category list admin-configurable would be additional scope tied to "Manage System Configuration" (F10) if confirmed.

## Technical hints (optional)

- `Category`/`Priority` are likely enums or small reference tables on the `Ticket` entity in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Admin-configurable category/priority lists (see Extra notes - open question).
