# Story intake

- Folder: `.squad/stories/customer-portal/track-ticket-requests/intake.md`

---

## Feature

- **Feature name (display):** F08 — Customer Portal
- **Feature slug (folder under `plans/`):** `customer-portal`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-048`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F08, customer-portal`

---

## Title

```
Track Requests
```

---

## Description

```
Source Requirements: FR-042
Feature: F08 - Customer Portal
Actor: Customer

Business goal:
As a Customer, I want to track the status of my submitted support requests so that
I know where they stand without contacting support again.

User Story: As a Customer, I want to track the status of my submitted support
requests, so that I know where they stand without contacting support again.

Business value: Directly reduces support-contact volume; core expected Portal
capability.

Priority: Must Have.

Business rules (from documentation):
- Customer can ... track requests (F08 acceptance criteria).
- A Customer must only see their own tickets, never another customer's.
```

---

## Acceptance criteria

```
Given an authenticated Customer
When they view their list of submitted tickets
Then only tickets belonging to them are returned, with current status

Given an authenticated Customer
When they view a specific ticket of theirs
Then its current details/status are returned (reusing "Track Ticket" data, scoped
  to their own record)

Given an authenticated Customer
When they attempt to view a ticket that does not belong to them
Then the request is forbidden/not-found (no information about other customers'
  tickets is leaked)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customer-portal/submit-ticket-via-customer-portal` and `tickets/track-ticket`'s underlying data.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- None beyond the authorization scoping above.

## Technical hints (optional)

- Customer-facing, authorization-scoped read view over the same `Ticket` data as `tickets/track-ticket`, filtered to `CustomerId = currentUser`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Staff-side ticket tracking across all customers (see "Track Ticket").
