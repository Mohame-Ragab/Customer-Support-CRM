# Story intake

- Folder: `.squad/stories/auth/reset-password/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-006`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Reset Password
```

---

## Description

```
Source Requirements: FR-006
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As a user with a valid password-reset link/token, I want to set a new password so
that I can log in again.

User Story: As a user with a valid password-reset link/token, I want to set a new
password, so that I can log in again.

Business value: Completes the recovery flow started by "Forgot Password".

Priority: Should Have - paired with Forgot Password; same rationale.

Business rules (from documentation):
- Users can ... recover/reset passwords (F00 acceptance criteria).
```

---

## Acceptance criteria

```
Given a valid, unexpired reset token and a new password meeting the password policy
When the user submits the reset
Then the account's password is updated
And the reset token is invalidated (single use)
And the user can subsequently log in with the new password

Given an expired, already-used, or invalid reset token
When a reset is attempted
Then it is rejected with a clear error and the password is not changed

Given a new password that does not meet the password policy
When a reset is attempted
Then it is rejected with a validation error describing the policy violation
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `auth/forgot-password` for the token.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- Password policy specifics beyond "valid credentials"/"password" are not detailed in the documentation; use the same policy as "Change Password" for consistency (kept as one shared policy across both stories rather than inventing two different ones).

## Technical hints (optional)

- ASP.NET Core Identity `ResetPasswordAsync` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Requesting the reset link (see "Forgot Password").
