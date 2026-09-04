# Story intake

- Folder: `.squad/stories/tickets/create-ticket/intake.md`

---

## Feature

- **Feature name (display):** F02 — Ticket Management
- **Feature slug (folder under `plans/`):** `tickets`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-025`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F02, tickets`

---

## Title

```
Create Ticket
```

---

## Description

```
Source Requirements: FR-015
Feature: F02 - Ticket Management (Classification: Source-derived)
Actor: Agent, Supervisor, Administrator (staff-side creation); a Customer creates a
  ticket via the Customer Portal (see `customer-portal/submit-ticket-via-customer-portal`,
  which depends on this story's underlying capability)

Business goal:
As staff, I want to create a support ticket for a customer so that a reported issue
is tracked in the system.

User Story: As staff, I want to create a support ticket for a customer, so that a
reported issue is tracked in the system.

Business value: The core object every other Ticket Management, Agent Dashboard,
SLA, and Customer Portal story operates on.

Priority: Must Have.

Business rules (from documentation):
- Ticket can be created and tracked (F02 acceptance criteria) - this story covers
  "created."
- A ticket must be associated with a customer (F01) to be meaningful, though the
  documentation does not state this explicitly - it is a reasonable, minimal
  inference given F01 exists as a prerequisite feature and Ticket Management's own
  description references customer support requests.
```

---

## Acceptance criteria

```
Given authenticated staff and an existing customer
When they submit a new ticket (subject/description at minimum - category, priority,
  and assignment are set via their own dedicated stories, not required at creation)
Then a new ticket is created in an initial status and returned

Given required fields (e.g. subject) are missing
When the create is submitted
Then it is rejected with a validation error

Given a ticket is created for a customer id that does not exist
When the create is submitted
Then it is rejected with a validation/not-found error
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customers/create-customer` existing (a ticket belongs to a customer).
- **Depends on code areas or other stories:** `tickets/set-ticket-category-and-priority`, `tickets/assign-ticket`, `tickets/change-ticket-status` build on the ticket this story creates; feeds `customers/view-customer-interaction-history`.

## Extra notes (optional)

- The exact minimum required field set beyond "subject/description" is not enumerated in the documentation.

## Technical hints (optional)

- New `Ticket` domain entity in `src/BackEnd` (Onion Architecture), referencing `Customer` by id. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Category/priority, assignment, status changes, escalation, and history are separate stories.
