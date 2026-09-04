# Story intake

- Folder: `.squad/stories/security-admin/manage-roles/intake.md`

---

## Feature

- **Feature name (display):** F10 — Security & Administration
- **Feature slug (folder under `plans/`):** `security-admin`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-011`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F10, security-admin`

---

## Title

```
Roles
```

---

## Description

```
Source Requirements: FR-052
Feature: F10 - Security & Administration
Actor: Administrator

Business goal:
As an Administrator, I want to manage roles and assign a role to a user so that
access across the CRM is governed by role (Cross-Feature Requirement:
"Authorization: Roles and permissions govern access across CRM capabilities").

User Story: As an Administrator, I want to manage roles and assign a role to a
user, so that access is governed by role across the CRM.

Business value: Cross-Feature Requirement: authorization is role/permission-driven
system-wide; nothing can be access-controlled without roles existing.

Priority: Must Have.

Business rules (from documentation):
- Users and roles can be managed (F10 acceptance criteria).
- F00's purpose names four actor types: Customer, Agent, Supervisor, Administrator.
```

---

## Acceptance criteria

```
Given an authenticated Administrator
When they view the list of roles
Then the roles are returned

Given an authenticated Administrator
When they assign a role to an existing user
Then the user's role assignment is updated and reflected on their next
  authentication (JWT role claim)

Given a non-Administrator
When they attempt to view or assign roles
Then the request is forbidden

Given an attempt to assign a role that does not exist
When it is submitted
Then it is rejected with a validation error
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** `security-admin/manage-users` (role is assigned to a user); permissions per role are `security-admin/manage-role-permissions`.

## Extra notes (optional)

- Open question / assumption: the documentation names Customer, Agent, Supervisor, Administrator as F00's actors, but the recommended-sequence text for F09 speaks of "management" visibility, and this project's backend already seeds a fourth staff role, "Manager," alongside Admin/Supervisor/Agent. The documentation does not explicitly reconcile "Supervisor" vs. "Manager" as the same or different roles. Flagged here and in `reports-management/management-dashboards` for product clarification rather than silently deciding.
- Whether roles are a fixed, predefined set (Customer/Agent/Supervisor/Administrator, matching F00) or an open, admin-defined set is not specified; the "Roles" FR wording ("can be managed") suggests admin-defined, but this may just mean "assigned." Flagged as open question.

## Technical hints (optional)

- `ApplicationRole`/`RoleManager<ApplicationRole>` already present in `src/BackEnd`, currently seeded with Admin/Supervisor/Manager/Agent (`Domain.Constants.Roles`) - reconcile with the open question above before finalizing which roles this story manages. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Fine-grained permission assignment within a role (see "Manage Role Permissions").
