# Story intake

- Folder: `.squad/stories/customer-portal/submit-ticket-via-customer-portal/intake.md`

---

## Feature

- **Feature name (display):** F08 — Customer Portal
- **Feature slug (folder under `plans/`):** `customer-portal`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-047`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F08, customer-portal`

---

## Title

```
Submit Tickets
```

---

## Description

```
Source Requirements: FR-041
Feature: F08 - Customer Portal (Classification: Source-derived)
Actor: Customer

Business goal:
As a Customer, I want to submit a support ticket through the portal so that I can
get help without contacting an Agent directly first.

User Story: As a Customer, I want to submit a support ticket through the portal,
so that I can get help without contacting an Agent directly first.

Business value: Core self-service value proposition of the Customer Portal - the
feature's namesake capability.

Priority: Must Have.

Business rules (from documentation):
- Customer can submit tickets (F08 acceptance criteria).
- Customer must be authenticated (logged in) and verified to use the portal, per
  F00's rule that customers access the portal after required verification.
```

---

## Acceptance criteria

```
Given an authenticated, verified Customer
When they submit a ticket (subject/description, reusing "Create Ticket")
Then a ticket is created, associated with them, and returned

Given required fields are missing
When the submission is made
Then it is rejected with a validation error

Given an unauthenticated visitor
When they attempt to submit a ticket through this authenticated portal capability
Then the request is rejected as unauthorized (see "Web Forms Channel" for the
  possible unauthenticated alternative - flagged as an open question there)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket` and `auth/user-login`/`auth/customer-registration`.
- **Depends on code areas or other stories:** see `communication-channels/web-forms-channel` for a flagged, unresolved overlap.

## Extra notes (optional)

- See `communication-channels/web-forms-channel`'s Extra notes for the open question about its relationship to this story.

## Technical hints (optional)

- Customer-facing, authorization-scoped wrapper around `tickets/create-ticket`'s API in `src/BackEnd`/`src/FrontEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Staff-side ticket creation (see "Create Ticket").
