# Story intake

- Folder: `.squad/stories/customers/create-customer/intake.md`

---

## Feature

- **Feature name (display):** F01 — Customer Management
- **Feature slug (folder under `plans/`):** `customers`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-019`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F01, customers`

---

## Title

```
Customer Profile - Create
```

---

## Description

```
Source Requirements: FR-010, FR-011
Feature: F01 - Customer Management (Classification: Source-derived)
Actor: Agent, Supervisor, Administrator (staff creating/maintaining a customer
  record - as distinct from the customer's own self-service registration, FR-002)

Business goal:
As staff, I want to create a customer profile (including contact details) so that
the customer's record exists in the CRM for ticket/interaction tracking.

User Story: As staff, I want to create a customer profile (including contact
details), so that the customer's record exists in the CRM for ticket/interaction
tracking.

Business value: Every ticket, note, attachment, and interaction-history story in
the system depends on a Customer record existing first.

Priority: Must Have.

Business rules (from documentation):
- Customer profiles can be created, viewed and updated (F01 acceptance criteria) -
  this story covers "created."
- Contact details are accessible (FR-011); modeled here as fields on the customer
  profile rather than a separate manageable entity, since the documentation gives
  no further detail distinguishing "Contact Details" from the "Customer Profile"
  itself.
```

---

## Acceptance criteria

```
Given authenticated staff (Agent, Supervisor, or Administrator)
When they submit a new customer's profile and contact details (the documentation
  does not enumerate the exact field list; minimum viable: name and at least one
  contact method, e.g. email)
Then a new customer profile is created and returned

Given required fields are missing or a contact detail is malformed (e.g. invalid
  email)
When the create is submitted
Then it is rejected with a validation error identifying the field(s)

Given a Customer (self-service) or unauthenticated user
When they attempt to create a customer profile through this staff-facing capability
Then the request is forbidden

Given the request is in Arabic or English
When a validation error is returned
Then it is localized accordingly
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** paired with `customers/view-customer` and `customers/update-customer` (same FR-010/011, split as a vertical CRUD slice per the task's own worked example for this feature).

## Extra notes (optional)

- Open question: whether a "Customer" created here is the same account a self-registering Customer (FR-002) creates for themselves, or whether staff-created "Customer" profiles are CRM records that may or may not be linked to a portal login, is not specified in the documentation. Both F00 (self-registration) and F01 (staff-managed profile) exist as separate features; flagged for product clarification on how/whether they reconcile to one record.

## Technical hints (optional)

- New `Customer` domain entity in `src/BackEnd` (Onion Architecture: entity + `IEntityTypeConfiguration` + generic repository + API), following the same pattern already established for Identity entities. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Interaction history, notes, and attachments (separate stories).
- Customer self-registration (`auth/customer-registration`).
