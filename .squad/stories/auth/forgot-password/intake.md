# Story intake

- Folder: `.squad/stories/auth/forgot-password/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-005`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Forgot Password
```

---

## Description

```
Source Requirements: FR-005
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As a user who cannot remember their password, I want to request a password reset
so that I can regain access to my account without contacting an administrator.

User Story: As a user who cannot remember their password, I want to request a
password reset, so that I can regain access to my account without contacting an
administrator.

Business value: Reduces support burden and account-lockout risk.

Priority: Should Have - important recovery path; the system remains usable without
it via admin-assisted reset in the interim.

Business rules (from documentation):
- Users can ... recover/reset passwords (F00 acceptance criteria).
```

---

## Acceptance criteria

```
Given a user submits their account's email address to the "forgot password" request
When the email corresponds to an existing account
Then a password-reset link/token is generated and sent to that email
And the response is the same regardless of whether the email exists, to avoid
  revealing which accounts are registered

Given a user submits an email that has no matching account
When the request is made
Then the system responds the same way as the success case (no account enumeration)
  and does not send anything
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** produces the token consumed by `auth/reset-password`.

## Extra notes (optional)

- Open question: reset-link/token expiry duration is not specified in the documentation.

## Technical hints (optional)

- ASP.NET Core Identity provides `GeneratePasswordResetTokenAsync` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Actually setting the new password — see "Reset Password".
