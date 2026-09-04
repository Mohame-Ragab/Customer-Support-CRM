# Story intake

- Folder: `.squad/stories/reports-management/ticket-reports/intake.md`

---

## Feature

- **Feature name (display):** F09 — Reports & Management
- **Feature slug (folder under `plans/`):** `reports-management`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-052`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F09, reports-management`

---

## Title

```
Ticket Reports
```

---

## Description

```
Source Requirements: FR-046
Feature: F09 - Reports & Management (Classification: Source-derived)
Actor: Supervisor, Administrator (see Extra notes re: "Manager" role)

Business goal:
As a Supervisor/Administrator, I want reports on tickets (volume, status
breakdown, category/priority breakdown) so that I have visibility into ticket
activity, per F09's purpose: "Provide management visibility into tickets."

User Story: As a Supervisor/Administrator, I want reports on tickets (volume,
status breakdown, category/priority breakdown), so that I have visibility into
ticket activity.

Business value: Explicitly named in F09's purpose; core management-visibility
capability once enough ticket data exists.

Priority: Should Have.

Business rules (from documentation):
- Ticket reports are reportable (F09 acceptance criteria).
```

---

## Acceptance criteria

```
Given tickets exist in the system
When an authorized user requests a ticket report (e.g. for a date range)
Then counts/breakdowns of tickets by status, category, and priority are returned

Given an Agent or Customer (not Supervisor/Administrator)
When they attempt to access ticket reports
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`, `tickets/change-ticket-status`, `tickets/set-ticket-category-and-priority`.
- **Depends on code areas or other stories:** feeds `reports-management/management-dashboards`.

## Extra notes (optional)

- See `reports-management/management-dashboards` for the flagged open question about the "Manager" role vs. the documentation's F00 actor list (Customer/Agent/Supervisor/Administrator).
- Open question: exact report dimensions/filters (date range, per-agent, per-department) are not enumerated in the documentation beyond "Ticket Reports."

## Technical hints (optional)

- Read-only aggregation queries over `Ticket` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- SLA/agent/customer-satisfaction-specific reports (separate stories, see below).
