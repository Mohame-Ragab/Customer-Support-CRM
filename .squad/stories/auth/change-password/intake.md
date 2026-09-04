# Story intake

- Folder: `.squad/stories/auth/change-password/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-007`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Change Password
```

---

## Description

```
Source Requirements: FR-007
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As an authenticated user, I want to change my password (knowing my current one)
so that I can maintain account security without going through the forgot-password
flow.

User Story: As an authenticated user, I want to change my password (knowing my
current one), so that I can maintain account security without going through the
forgot-password flow.

Business value: Standard account-security hygiene capability.

Priority: Should Have - valuable, not launch-blocking.

Business rules (from documentation):
- Authorized users can update password and profile (F00 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated user who knows their current password
When they submit their current password and a new password meeting the password
  policy
Then the password is updated
And existing sessions/tokens are not silently invalidated unless that is also true
  for other password-changing flows (kept consistent with Reset Password)

Given the submitted current password does not match the account's actual password
When a change is attempted
Then it is rejected with an authentication error and the password is unchanged

Given the new password does not meet the password policy, or matches the current
  password
When a change is attempted
Then it is rejected with a validation error
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** requires `auth/user-login` (must be authenticated).
- **Depends on code areas or other stories:** shares password-policy rules with `auth/reset-password`.

## Extra notes (optional)

- Same password-policy caveat as "Reset Password" — exact policy is not enumerated in the documentation.

## Technical hints (optional)

- ASP.NET Core Identity `ChangePasswordAsync` in `src/BackEnd`; surfaced from the "User Profile" screen per F00's acceptance criteria grouping ("update password and profile"). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Forgot/Reset password (separate stories, for users who don't know their current password).
