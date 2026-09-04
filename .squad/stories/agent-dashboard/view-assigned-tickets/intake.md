# Story intake

- Folder: `.squad/stories/agent-dashboard/view-assigned-tickets/intake.md`

---

## Feature

- **Feature name (display):** F04 — Agent Dashboard
- **Feature slug (folder under `plans/`):** `agent-dashboard`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-035`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F04, agent-dashboard`

---

## Title

```
Assigned Tickets
```

---

## Description

```
Source Requirements: FR-026
Feature: F04 - Agent Dashboard (Classification: Source-derived)
Actor: Agent

Business goal:
As an Agent, I want to see the tickets assigned to me in one place so that I have a
clear, unified view of my workload.

User Story: As an Agent, I want to see the tickets assigned to me in one place, so
that I have a clear, unified view of my workload.

Business value: Primary daily entry point for every Agent - core to the Agent
Dashboard's purpose.

Priority: Must Have.

Business rules (from documentation):
- Agents can see assigned tickets (F04 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Agent
When they open their dashboard
Then the tickets currently assigned to them are listed

Given an Agent has no tickets assigned
When they open their dashboard
Then an empty state is shown (not an error)

Given a Supervisor or Administrator
When they view an Agent's assigned tickets (not just their own)
Then the documentation does not specify this capability; scoped here to an Agent's
  own assigned tickets only, per FR-026's wording ("Agents can see assigned
  tickets")
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/assign-ticket` (and/or `sla-automation/automatic-ticket-assignment`).
- **Depends on code areas or other stories:** uses `tickets/track-ticket`'s underlying data.

## Extra notes (optional)

- None beyond the scope note above.

## Technical hints (optional)

- A filtered query over `Ticket` by `AssignedAgentId = currentUser.Id`, using the generic repository already established in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- A Supervisor/Administrator view of all agents' assigned tickets (would be a Management Dashboard / reporting concern - see F09 - not this story).
