# Story intake

- Folder: `.squad/stories/security-admin/manage-role-permissions/intake.md`

---

## Feature

- **Feature name (display):** F10 — Security & Administration
- **Feature slug (folder under `plans/`):** `security-admin`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-012`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F10, security-admin`

---

## Title

```
Permissions
```

---

## Description

```
Source Requirements: FR-053
Feature: F10 - Security & Administration
Actor: Administrator

Business goal:
As an Administrator, I want to assign permissions to roles so that access to
specific CRM capabilities can be governed more granularly than by role alone.

User Story: As an Administrator, I want to assign permissions to roles, so that
access to specific CRM capabilities can be governed more granularly than by role
alone.

Business value: Named cross-feature authorization requirement, but role-based
[Authorize(Roles=...)] can operate the system without this initially.

Priority: Should Have.

Business rules (from documentation):
- Permissions can be assigned (F10 acceptance criteria).
- Cross-Feature Requirement: Authorization - roles and permissions govern access
  across CRM capabilities.
```

---

## Acceptance criteria

```
Given an authenticated Administrator
When they view the permissions currently assigned to a role
Then the assigned permissions are returned

Given an authenticated Administrator
When they assign or remove a permission for a role
Then the change takes effect for users holding that role

Given a non-Administrator
When they attempt to view or change role permissions
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `security-admin/manage-roles` (permissions attach to roles).
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- Open question / assumption: the documentation does not enumerate the concrete list of permissions (what a "permission" corresponds to per screen/action), nor whether permissions attach to roles or to individual users. Assumed here: permissions attach to roles (consistent with FR-052 "Roles" immediately preceding FR-053 "Permissions" in the same feature, and with role-based `[Authorize(Roles = ...)]` already used in the existing backend). The concrete permission catalog must be defined before implementation - this is a genuine open question, not silently resolved.

## Technical hints (optional)

- The existing backend (`src/BackEnd`) currently enforces authorization via ASP.NET Core's role-based `[Authorize(Roles = ...)]`; a distinct, admin-configurable permission catalog beyond roles is new scope this story introduces. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Defining the exact permission catalog is a product decision that should precede detailed implementation planning of this story.
