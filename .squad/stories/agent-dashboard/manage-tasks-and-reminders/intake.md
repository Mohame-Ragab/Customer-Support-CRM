# Story intake

- Folder: `.squad/stories/agent-dashboard/manage-tasks-and-reminders/intake.md`

---

## Feature

- **Feature name (display):** F04 — Agent Dashboard
- **Feature slug (folder under `plans/`):** `agent-dashboard`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-037`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F04, agent-dashboard`

---

## Title

```
Tasks & Reminders
```

---

## Description

```
Source Requirements: FR-028
Feature: F04 - Agent Dashboard
Actor: Agent

Business goal:
As an Agent, I want to create tasks/reminders for myself so that I don't lose
track of follow-ups I need to do.

User Story: As an Agent, I want to create tasks/reminders for myself, so that I
don't lose track of follow-ups I need to do.

Business value: Personal productivity aid; no other story depends on it.

Priority: Could Have.

Business rules (from documentation):
- Tasks/reminders ... are supported (F04 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Agent
When they create a task/reminder with a description and a due date/time
Then it is saved and appears on their dashboard

Given an authenticated Agent
When they view their tasks/reminders
Then their own tasks/reminders are listed (not other agents')

Given a task/reminder's due date/time has passed
When the Agent views their dashboard
Then it is visibly indicated as due/overdue (documentation does not specify a push
  notification for this - see "System Notifications" for that separate capability)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none required, though a task is often linked to a ticket in practice.
- **Depends on code areas or other stories:** may relate to `sla-automation/system-notifications` for due-reminder alerts, if confirmed.

## Extra notes (optional)

- Open question: whether a task/reminder must be linked to a specific ticket, or can be a standalone personal to-do, is not specified in the documentation. Scoped to standalone-capable (optionally linkable to a ticket) as the more general, minimum-viable interpretation.

## Technical hints (optional)

- New `AgentTask` (or similar) entity in `src/BackEnd`, scoped to the owning Agent (`ICurrentUserService`). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Proactive push/email notification when a reminder is due (see "System Notifications" - open question on overlap).
