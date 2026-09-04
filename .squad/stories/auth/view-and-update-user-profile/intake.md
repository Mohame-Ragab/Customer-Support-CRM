# Story intake

- Folder: `.squad/stories/auth/view-and-update-user-profile/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-009`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
User Profile
```

---

## Description

```
Source Requirements: FR-009
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As an authenticated user, I want to view and update my own profile so that my
account information stays accurate.

User Story: As an authenticated user, I want to view and update my own profile, so
that my account information stays accurate.

Business value: Basic account self-service expectation.

Priority: Should Have.

Business rules (from documentation):
- Authorized users can update password and profile (F00 acceptance criteria).
- A user may view/update only their own profile via this story; an Administrator
  managing other users' accounts is the separate "Manage Users" (F10, FR-051) story.
```

---

## Acceptance criteria

```
Given an authenticated user
When they request their own profile
Then their profile information is returned (excluding password/credential data)

Given an authenticated user submits a profile update (e.g. name, contact
  information - the documentation does not enumerate the exact editable field set)
When the update is valid
Then the profile is updated and the updated profile is returned

Given a profile update includes an invalid value (e.g. malformed email)
When it is submitted
Then it is rejected with a validation error identifying the field(s)

Given a user attempts to view or update another user's profile through this
  endpoint
When the request is made
Then it is forbidden (self-service profile only)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** requires `auth/user-login`.
- **Depends on code areas or other stories:** password change is handled by `auth/change-password`, not this story.

## Extra notes (optional)

- Open question: the exact set of editable profile fields is not specified beyond "User Profile" — implement the minimum viable field set present on the account (name, email/contact) and extend later if a more specific field list is provided.

## Technical hints (optional)

- Backed by `ApplicationUser`/`ICurrentUserService` already present in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Password change (separate story).
- Role/permission changes (Administrator-only, covered by "Manage Users"/"Manage Roles").
