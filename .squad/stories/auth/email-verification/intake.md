# Story intake

- Folder: `.squad/stories/auth/email-verification/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-004`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Email Verification
```

---

## Description

```
Source Requirements: FR-004
Feature: F00 - Authentication & Registration
Actor: Customer

Business goal:
As a newly registered Customer, I want to verify my email address so that I can
gain full access to the Customer Portal, per F00's rule that customers access the
portal "after required verification."

User Story: As a newly registered Customer, I want to verify my email address, so
that I can gain full access to the Customer Portal.

Business value: The documentation makes portal access explicitly conditional on
verification.

Priority: Must Have - explicitly gates Customer Portal access per F00's own
acceptance criteria.

Business rules (from documentation):
- Customers can register and access the portal after required verification.
```

---

## Acceptance criteria

```
Given a Customer has registered and received a verification link/token
When they open the verification link (or submit the verification token) before it
  expires
Then their account is marked verified
And they can subsequently log in with full Customer Portal access

Given a verification link/token has already been used, or has expired, or does not
  match any pending registration
When it is submitted
Then verification fails with a clear error, and the user is not marked verified

Given an unverified Customer requests the verification email/link again
When they request it
Then a new verification link/token is issued (invalidating any prior unused one)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `auth/customer-registration`.
- **Depends on code areas or other stories:** gates full access in `auth/user-login`.

## Extra notes (optional)

- Open question: the delivery mechanism (email) implies the "Email Channel" (FR-023) or a dedicated transactional-email capability; the documentation does not specify which. Treated here as a minimal, dedicated email-send for the verification link, independent of the F03 Email communication channel used for support conversations.
- Open question: verification link/token expiry duration is not specified in the documentation.

## Technical hints (optional)

- ASP.NET Core Identity provides built-in email-confirmation token support (`UserManager.GenerateEmailConfirmationTokenAsync` / `ConfirmEmailAsync`) in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- The email-sending infrastructure itself, if not already covered by "Email Communication Channel" (FR-023) — flag for coordination with that story rather than duplicating an email-sending mechanism.
