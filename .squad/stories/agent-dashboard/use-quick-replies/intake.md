# Story intake

- Folder: `.squad/stories/agent-dashboard/use-quick-replies/intake.md`

---

## Feature

- **Feature name (display):** F04 — Agent Dashboard
- **Feature slug (folder under `plans/`):** `agent-dashboard`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-038`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F04, agent-dashboard`

---

## Title

```
Quick Replies
```

---

## Description

```
Source Requirements: FR-029
Feature: F04 - Agent Dashboard
Actor: Agent

Business goal:
As an Agent, I want to save and reuse canned response templates ("quick replies")
so that I can respond to common questions faster.

User Story: As an Agent, I want to save and reuse canned response templates
("quick replies"), so that I can respond to common questions faster.

Business value: Efficiency aid; no other story depends on it.

Priority: Could Have.

Business rules (from documentation):
- Quick replies are supported (F04 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Agent
When they create a quick reply template (a short name and the reply text)
Then it is saved and available for reuse

Given an authenticated Agent replying on a ticket (e.g. via the Email channel)
When they select one of their quick replies
Then its text is inserted into the reply they are composing

Given an authenticated Agent
When they view their list of quick replies
Then their saved templates are returned
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none required to create/manage templates; using one in a reply depends on `communication-channels/email-communication-channel` or `communication-channels/live-chat-communication-channel` existing.
- **Depends on code areas or other stories:** see above.

## Extra notes (optional)

- Open question: whether quick replies are personal to each Agent or shared/team-level (e.g. defined by a Supervisor for the whole team) is not specified in the documentation. Scoped to per-Agent, personal templates as the minimum viable interpretation; a shared/team library would be additional scope pending clarification.

## Technical hints (optional)

- New `QuickReplyTemplate` entity in `src/BackEnd`, scoped to the owning Agent. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Shared/team-level template libraries (see Extra notes - open question).
