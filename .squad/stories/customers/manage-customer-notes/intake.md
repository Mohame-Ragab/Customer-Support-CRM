# Story intake

- Folder: `.squad/stories/customers/manage-customer-notes/intake.md`

---

## Feature

- **Feature name (display):** F01 — Customer Management
- **Feature slug (folder under `plans/`):** `customers`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-023`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F01, customers`

---

## Title

```
Customer Notes
```

---

## Description

```
Source Requirements: FR-013
Feature: F01 - Customer Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to add and view internal notes on a customer's record so that
context useful to future interactions is captured and shared across the team.

User Story: As staff, I want to add and view internal notes on a customer's
record, so that context useful to future interactions is captured and shared
across the team.

Business value: Improves continuity of service; not required for a ticket to be
handled at all.

Priority: Should Have.

Business rules (from documentation):
- Notes ... can be managed (F01 acceptance criteria).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing customer
When they add a note with text content
Then the note is saved against that customer, attributed to the author and timestamp

Given authenticated staff and an existing customer
When they view that customer's notes
Then the notes are returned in chronological order with author and timestamp

Given a note submission with empty content
When it is submitted
Then it is rejected with a validation error

Given a Customer (self-service) or unauthenticated user
When they attempt to view or add notes through this staff-facing capability
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customers/create-customer`.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- Open question: the documentation says notes "can be managed" without specifying whether edit/delete of an existing note is included, or whether notes are append-only for audit purposes. Scoped here to add + view (the minimum unambiguous reading); edit/delete needs product clarification before being added.

## Technical hints (optional)

- New `CustomerNote` entity in `src/BackEnd`, following the same Onion Architecture pattern as `Customer`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Edit/delete of existing notes (see Extra notes - open question).
