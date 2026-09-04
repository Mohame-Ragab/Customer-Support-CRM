# Story intake

- Folder: `.squad/stories/reports-management/management-dashboards/intake.md`

---

## Feature

- **Feature name (display):** F09 — Reports & Management
- **Feature slug (folder under `plans/`):** `reports-management`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-056`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F09, reports-management`

---

## Title

```
Management Dashboards
```

---

## Description

```
Source Requirements: FR-050
Feature: F09 - Reports & Management
Actor: Supervisor, Administrator (see Extra notes - "Manager" open question)

Business goal:
As a Supervisor/Administrator, I want a single dashboard that surfaces ticket, SLA,
agent performance, and customer satisfaction data at a glance so that I don't have
to open each report individually.

User Story: As a Supervisor/Administrator, I want a single dashboard that
surfaces ticket, SLA, agent performance, and customer satisfaction data at a
glance, so that I don't have to open each report individually.

Business value: Composition/convenience layer on top of the four report stories -
the highest-level management-visibility capability named in F09's purpose.

Priority: Should Have.

Business rules (from documentation):
- Management dashboards are available (F09 acceptance criteria).
```

---

## Acceptance criteria

```
Given the underlying reports exist (Ticket Reports, Agent
  Performance, Customer Satisfaction)
When an authorized user opens the management dashboard
Then summary data from each of those reports is displayed together on one screen

Given an Agent or Customer
When they attempt to access the management dashboard
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `reports-management/ticket-reports`, `reports-management/agent-performance-reports`, `reports-management/customer-satisfaction-reports`. (Formerly also depended on `reports-management/sla-performance-reports`, FR-047, removed from scope.)
- **Depends on code areas or other stories:** this is a composition/aggregation story on top of the four report stories above; it does not introduce new report data of its own.

## Extra notes (optional)

- OPEN QUESTION (project-wide, most visible here): F00's actor list is Customer, Agent, Supervisor, Administrator. F09's feature name is "Reports & **Management**" and its purpose is "management visibility." Separately, this project's existing backend already seeds a fourth staff role, "Manager" (alongside Admin/Supervisor/Agent), that is not named anywhere in this requirements document. The documentation does not reconcile whether "Supervisor" is meant to be the "management" role referenced by F09, whether "Manager" is a distinct role the documentation simply omitted, or whether "Manager" should be removed/renamed to align with the documentation. This affects role/authorization decisions across every story that restricts access to "Supervisor, Administrator" throughout this feature set (F09, and parts of F05/F10). Flagged here for explicit product/stakeholder clarification before authorization is finalized on any of these stories - not silently resolved.

## Technical hints (optional)

- Frontend composition (`src/FrontEnd`) over the four report APIs; no new backend data beyond what those stories already expose. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Any report data not already produced by the four dependency stories listed above.
