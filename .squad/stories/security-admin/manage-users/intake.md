# Story intake

- Folder: `.squad/stories/security-admin/manage-users/intake.md`

---

## Feature

- **Feature name (display):** F10 — Security & Administration
- **Feature slug (folder under `plans/`):** `security-admin`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-010`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F10, security-admin`

---

## Title

```
Users
```

---

## Description

```
Source Requirements: FR-051
Feature: F10 - Security & Administration (Classification: Source-derived)
Actor: Administrator

Business goal:
As an Administrator, I want to manage user accounts (create, view, update) so that
staff (Agents, Supervisors, other Administrators) have accounts to work with the
system.

User Story: As an Administrator, I want to manage user accounts (create, view,
update), so that staff have accounts to work with the system.

Business value: No staff can be onboarded onto the CRM without this - blocks every
staff-facing story.

Priority: Must Have.

Business rules (from documentation):
- Users and roles can be managed (F10 acceptance criteria).
- Authorization: Roles and permissions govern access across CRM capabilities
  (Cross-Feature Requirements) - only an Administrator performs this management.
```

---

## Acceptance criteria

```
Given an authenticated Administrator
When they submit valid details for a new staff user account (name, email, initial
  role)
Then the account is created and can subsequently authenticate (see "User Login")

Given an authenticated Administrator
When they request the list of users, or a specific user's details
Then the user list/details are returned

Given an authenticated Administrator
When they update an existing user's details
Then the update is applied and the updated user is returned

Given a non-Administrator (Agent, Supervisor, or Customer)
When they attempt to create, list, view, or update a user account through this
  capability
Then the request is forbidden

Given invalid or duplicate data (e.g. an email already in use)
When a create/update is submitted
Then it is rejected with a validation/conflict error
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** role assignment on a user depends on `security-admin/manage-roles`; created accounts authenticate via `auth/user-login`.

## Extra notes (optional)

- Open question / assumption: "Users" (F10) is interpreted as Administrator-managed staff accounts (Agent/Supervisor/Administrator), distinct from self-service "Customer Registration" (FR-002). The documentation does not explicitly draw this line; flagging for product confirmation.
- Open question: the documentation does not describe an explicit activate/deactivate or delete action for users (only "managed") - not included as a separate acceptance criterion to avoid inventing scope; revisit if the product intends account suspension.

## Technical hints (optional)

- Builds on `ApplicationUser`/`RoleManager` already present in `src/BackEnd`'s Identity infrastructure. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Role/permission definitions themselves (see "Manage Roles", "Manage Role Permissions").
- Customer self-registration (separate story).
