# Story intake

- Folder: `.squad/stories/customer-portal/submit-customer-feedback/intake.md`

---

## Feature

- **Feature name (display):** F08 — Customer Portal
- **Feature slug (folder under `plans/`):** `customer-portal`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-051`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F08, customer-portal`

---

## Title

```
Submit Feedback
```

---

## Description

```
Source Requirements: FR-045
Feature: F08 - Customer Portal
Actor: Customer

Business goal:
As a Customer, I want to submit feedback (e.g. after my ticket is resolved) so
that the organization knows how satisfied I was with the support I received.

User Story: As a Customer, I want to submit feedback (e.g. after my ticket is
resolved), so that the organization knows how satisfied I was with the support I
received.

Business value: Sole data source for "Customer Satisfaction Reports" (F09) -
without this story, that report can never be populated.

Priority: Should Have.

Business rules (from documentation):
- Customer can ... submit feedback (F08 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Customer with a ticket (typically resolved/closed)
When they submit feedback for that ticket
Then the feedback is saved and associated with the ticket/customer

Given feedback is submitted without required content (e.g. a rating, if that is the
  chosen structure - see Extra notes)
When it is submitted
Then it is rejected with a validation error

Given an authenticated Customer attempts to submit feedback for a ticket that is
  not theirs
When it is submitted
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`/`customer-portal/submit-ticket-via-customer-portal`.
- **Depends on code areas or other stories:** feeds `reports-management/customer-satisfaction-reports`, which needs this feedback data as its source.

## Extra notes (optional)

- Open question: the exact feedback structure (a numeric/star rating, free text, or both) is not specified in the documentation. This directly affects what "Customer Satisfaction Reports" (F09/FR-049) can report on, so the structure should be confirmed before either story is implemented in detail.

## Technical hints (optional)

- New `CustomerFeedback` entity tied to `Ticket`/`Customer` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Aggregating/reporting on feedback (see "Customer Satisfaction Reports").
