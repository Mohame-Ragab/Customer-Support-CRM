# Story intake

- Folder: `.squad/stories/customers/view-customer/intake.md`

---

## Feature

- **Feature name (display):** F01 — Customer Management
- **Feature slug (folder under `plans/`):** `customers`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-020`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F01, customers`

---

## Title

```
Customer Profile - View
```

---

## Description

```
Source Requirements: FR-010, FR-011
Feature: F01 - Customer Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to view a customer's profile and contact details so that I have
the context needed to assist them.

User Story: As staff, I want to view a customer's profile and contact details, so
that I have the context needed to assist them.

Business value: Staff cannot do their job without being able to see who they're
helping.

Priority: Must Have.

Business rules (from documentation):
- Customer profiles can be created, viewed and updated (F01 acceptance criteria) -
  this story covers "viewed."
- Contact details are accessible (FR-011).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing customer id
When they request that customer's profile
Then the profile and contact details are returned

Given authenticated staff
When they request a list of customers
Then a list of customer profiles is returned

Given a customer id that does not exist
When it is requested
Then a not-found error is returned

Given a Customer (self-service) or unauthenticated user
When they attempt to view another customer's profile through this staff-facing
  capability
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customers/create-customer` (a customer must exist to be viewed).
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- The documentation does not mention a "search customers" capability explicitly; a basic list (not a search/filter capability) is the minimum viable scope of "viewed." If search is needed, it should be raised as its own requirement rather than assumed here.

## Technical hints (optional)

- Uses the generic repository's `GetByIdAsync`/`GetAllAsync` already established in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Search/filtering (not in the documentation - see Extra notes).
- Interaction history, notes, attachments (separate stories).
