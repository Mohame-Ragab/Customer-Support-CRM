# Story intake

- Folder: `.squad/stories/auth/customer-registration/intake.md`
- Binaries: put them in `attachments/` next to this file and list them below.
- Do **not** rely on external links — paste the content you want considered.

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-002`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Customer Registration
```

---

## Description

```
Source Requirements: FR-002
Feature: F00 - Authentication & Registration
Actor: Customer

Business goal:
As a prospective Customer, I want to register for an account so that I can access
the Customer Portal once my account is verified.

User Story: As a prospective Customer, I want to register for an account, so that
I can access the Customer Portal once my account is verified.

Business value: Customer self-service (Portal, ticket submission) has no user base
without this.

Priority: Must Have - required before any Customer Portal capability can be used.

Business rules (from documentation):
- Customers can register and access the portal after required verification
  (verification itself is the separate "Email Verification" story, FR-004).
- Registration is for the Customer role; F00's purpose distinguishes Customers from
  staff (Agent/Supervisor/Administrator), whose accounts are provisioned via
  "Manage Users" (F10, FR-051), not self-registration.
```

---

## Acceptance criteria

```
Given a prospective customer provides the required registration details (the
  documentation does not enumerate the exact field list beyond "Customer
  Registration" - use the minimum needed to authenticate: name, email, password)
When they submit the registration form
Then a Customer account is created in an unverified state
And a verification step is triggered (see "Email Verification")
And the raw password is never returned or logged

Given a registration is submitted with a missing required field or an invalid
  email format
When the form is submitted
Then the registration is rejected with a validation error identifying the field(s)

Given a registration is submitted with an email that already has an account
When the form is submitted
Then the registration is rejected as a duplicate, without revealing further detail
  about the existing account

Given the request is in Arabic or English (Accept-Language header)
When a validation or duplicate error is returned
Then the error message is localized accordingly
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** feeds into `auth/email-verification`; a registered Customer later authenticates via `auth/user-login`.

## Extra notes (optional)

- Open question: the exact registration field set (e.g., phone number, company) is not specified in the documentation. Minimum viable field set (name, email, password) is assumed; if F01 "Customer Profile" fields are meant to be captured at registration time, that needs product clarification.

## Technical hints (optional)

- Built on the existing ASP.NET Core Identity setup in `src/BackEnd` (`ApplicationUser`, role seeding already in place for Admin/Supervisor/Manager/Agent — a `Customer` role convention needs to be added consistently). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Email verification itself (separate story).
- Agent/Supervisor/Administrator account provisioning (see "Manage Users").
