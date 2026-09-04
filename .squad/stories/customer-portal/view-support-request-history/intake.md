# Story intake

- Folder: `.squad/stories/customer-portal/view-support-request-history/intake.md`

---

## Feature

- **Feature name (display):** F08 — Customer Portal
- **Feature slug (folder under `plans/`):** `customer-portal`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-049`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F08, customer-portal`

---

## Title

```
View History
```

---

## Description

```
Source Requirements: FR-043
Feature: F08 - Customer Portal
Actor: Customer

Business goal:
As a Customer, I want to view my past support request history so that I can refer
back to previous issues and their resolutions.

User Story: As a Customer, I want to view my past support request history, so
that I can refer back to previous issues and their resolutions.

Business value: Valuable self-service capability, secondary to submitting/tracking
active requests.

Priority: Should Have.

Business rules (from documentation):
- Customer can ... view history (F08 acceptance criteria).
- Scoped to the Customer's own history only.
```

---

## Acceptance criteria

```
Given an authenticated Customer
When they view their support history
Then their past (including resolved/closed) tickets are listed, most recent first

Given an authenticated Customer with no past tickets
When they view their history
Then an empty state is returned (not an error)

Given an authenticated Customer
When they attempt to view another customer's history
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customer-portal/submit-ticket-via-customer-portal`.
- **Depends on code areas or other stories:** conceptually related to `customers/view-customer-interaction-history` (the staff-facing equivalent) - same underlying data, different actor/authorization scope; not duplicated as a separate data model.

## Extra notes (optional)

- None beyond the relationship to `customers/view-customer-interaction-history` noted above.

## Technical hints (optional)

- Customer-facing, authorization-scoped read view; may reuse the same query shape as `customers/view-customer-interaction-history` with a different authorization filter. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Staff-facing interaction history across all customers (see "View Customer Interaction History").
