# Story intake

- Folder: `.squad/stories/security-admin/view-audit-logs/intake.md`

---

## Feature

- **Feature name (display):** F10 — Security & Administration
- **Feature slug (folder under `plans/`):** `security-admin`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-013`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F10, security-admin`

---

## Title

```
Audit Logs
```

---

## Description

```
Source Requirements: FR-054
Feature: F10 - Security & Administration
Actor: Administrator

Business goal:
As an Administrator, I want administrative operations to be recorded and viewable
as audit logs so that relevant actions are traceable, per the Cross-Feature
Requirement: "Auditability: Relevant administrative operations should be traceable
through audit logs."

User Story: As an Administrator, I want administrative operations to be recorded
and viewable as audit logs, so that relevant actions are traceable.

Business value: Named cross-feature Auditability requirement; important for
compliance/trust, not functionally blocking.

Priority: Should Have.

Business rules (from documentation):
- Audit logs are recorded (F10 acceptance criteria).
- Auditability applies to "relevant administrative operations" specifically (not
  necessarily every read/write in the system).
```

---

## Acceptance criteria

```
Given an Administrator performs an administrative operation covered by this story's
  initial scope (user create/update, role/permission change, system configuration
  change - i.e. the F10 capabilities themselves)
When the operation completes
Then an audit log entry is recorded capturing who performed it, what it was, and
  when (UTC)

Given an authenticated Administrator
When they view the audit log
Then recorded entries are returned, most recent first

Given a non-Administrator
When they attempt to view audit logs
Then the request is forbidden

Given an audit log entry is recorded
When it is stored
Then it does not include sensitive values such as passwords or tokens
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none to build the viewer itself; the set of "relevant administrative operations" logged will grow as `security-admin/manage-users`, `security-admin/manage-roles`, `security-admin/manage-role-permissions`, and `security-admin/manage-system-configuration` are implemented.
- **Depends on code areas or other stories:** see above.

## Extra notes (optional)

- Open question / assumption: the documentation does not enumerate which specific operations must be audited beyond "relevant administrative operations." Scoped here to F10's own capabilities (user/role/permission/config changes) as the minimum defensible interpretation; expanding audit coverage to non-F10 actions (e.g. ticket status changes) is not claimed as covered by this story unless clarified.

## Technical hints (optional)

- The existing `BaseEntity` audit fields (`CreatedBy`/`UpdatedBy`/timestamps, via `ICurrentUserService`) in `src/BackEnd` cover per-record change attribution already; a dedicated audit-log *feed* (who did what, when, across actions) is additional, purpose-built scope this story introduces. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Audit coverage of every feature in the system (see Extra notes) - only F10's own administrative operations are in scope for this story.
