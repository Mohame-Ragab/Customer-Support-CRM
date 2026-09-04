# Story intake

- Folder: `.squad/stories/customers/update-customer/intake.md`

---

## Feature

- **Feature name (display):** F01 — Customer Management
- **Feature slug (folder under `plans/`):** `customers`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-021`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F01, customers`

---

## Title

```
Customer Profile - Update
```

---

## Description

```
Source Requirements: FR-010, FR-011
Feature: F01 - Customer Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to update a customer's profile and contact details so that the
record stays accurate over time.

User Story: As staff, I want to update a customer's profile and contact details,
so that the record stays accurate over time.

Business value: Important data-quality capability, not blocking for initial ticket
handling.

Priority: Should Have.

Business rules (from documentation):
- Customer profiles can be created, viewed and updated (F01 acceptance criteria) -
  this story covers "updated."
```

---

## Acceptance criteria

```
Given authenticated staff and an existing customer
When they submit valid updated profile/contact details
Then the profile is updated and the updated profile is returned

Given an update includes an invalid value (e.g. malformed email)
When it is submitted
Then it is rejected with a validation error identifying the field(s)

Given a customer id that does not exist
When an update is attempted
Then a not-found error is returned

Given a Customer (self-service) or unauthenticated user
When they attempt to update a customer profile through this staff-facing capability
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

- None beyond those already noted on "Create Customer".

## Technical hints (optional)

- Uses the generic repository's `Update` + `IUnitOfWork.SaveChangesAsync` already established in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Interaction history, notes, attachments (separate stories).
