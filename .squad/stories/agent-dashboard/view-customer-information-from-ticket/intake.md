# Story intake

- Folder: `.squad/stories/agent-dashboard/view-customer-information-from-ticket/intake.md`

---

## Feature

- **Feature name (display):** F04 — Agent Dashboard
- **Feature slug (folder under `plans/`):** `agent-dashboard`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-036`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F04, agent-dashboard`

---

## Title

```
Customer Information
```

---

## Description

```
Source Requirements: FR-027
Feature: F04 - Agent Dashboard
Actor: Agent

Business goal:
As an Agent working a ticket, I want to see the related customer's information
alongside it so that I don't have to leave my workspace to get context.

User Story: As an Agent working a ticket, I want to see the related customer's
information alongside it, so that I don't have to leave my workspace to get
context.

Business value: Efficiency improvement over navigating separately to Customer
Management; workable without it, just slower.

Priority: Should Have.

Business rules (from documentation):
- Agents can see ... customer information (F04 acceptance criteria).
```

---

## Acceptance criteria

```
Given an Agent is viewing a ticket assigned to them
When they view the ticket
Then the associated customer's profile/contact details (from "View Customer") are
  shown alongside it

Given the ticket's customer also has notes/attachments/interaction history
When the Agent views the ticket
Then that context is reachable from the same workspace (linking to, not duplicating,
  `customers/view-customer-interaction-history`, `customers/manage-customer-notes`,
  `customers/manage-customer-attachments`)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customers/view-customer` and `tickets/track-ticket`.
- **Depends on code areas or other stories:** surfaces (does not duplicate) `customers/view-customer-interaction-history`, `customers/manage-customer-notes`, `customers/manage-customer-attachments`.

## Extra notes (optional)

- This is primarily a frontend composition of already-existing data (Customer + Ticket), not a new backend data model.

## Technical hints (optional)

- Frontend composition story on `src/FrontEnd`, combining existing Customer/Ticket API responses into one Agent-facing workspace view. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Editing the customer's profile from within this view is covered by `customers/update-customer`, not duplicated here.
