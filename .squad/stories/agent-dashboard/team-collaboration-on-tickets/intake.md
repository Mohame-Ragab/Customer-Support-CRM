# Story intake

- Folder: `.squad/stories/agent-dashboard/team-collaboration-on-tickets/intake.md`

---

## Feature

- **Feature name (display):** F04 — Agent Dashboard
- **Feature slug (folder under `plans/`):** `agent-dashboard`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-039`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F04, agent-dashboard`

---

## Title

```
Team Collaboration
```

---

## Description

```
Source Requirements: FR-030
Feature: F04 - Agent Dashboard
Actor: Agent, Supervisor

Business goal:
As an Agent, I want to collaborate with my team on a ticket so that I can get help
or share context without leaving the CRM.

User Story: As an Agent, I want to collaborate with my team on a ticket, so that I
can get help or share context without leaving the CRM.

Business value: Cannot be confidently assessed - the documentation gives no
elaboration beyond the feature title, so the cost/value of the actual deliverable
is unknown until scope is clarified (could be a one-line internal-comment field or
a full chat subsystem).

Priority: Needs Clarification - resolve the scope ambiguity (see Extra notes)
before this can be prioritized or estimated.

Business rules (from documentation):
- Team collaboration is supported (F04 acceptance criteria).

IMPORTANT - this requirement is significantly underspecified in the source
documentation. "Team Collaboration" has no further description anywhere in the
document beyond its own title and the one acceptance-criteria sentence. Per the
task's rule 10 ("If something is ambiguous, do not silently invent an assumption"),
this story is scoped to the narrowest, most defensible interpretation and the
ambiguity is flagged explicitly rather than guessed away.
```

---

## Acceptance criteria

```
Given authenticated staff (Agent or Supervisor) viewing a ticket
When they add an internal comment on that ticket (visible to staff only, not the
  customer)
Then the comment is saved and visible to other staff viewing the same ticket

Given a ticket has internal comments
When staff view the ticket
Then the comments are shown in chronological order with author and timestamp
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- OPEN QUESTION (significant): "Team Collaboration" could reasonably mean internal ticket comments (implemented here as the minimum viable, defensible scope), @mentions/notifications to specific teammates, presence/who's-viewing indicators, or a broader team chat feature. The documentation gives no elaboration to distinguish between these. This story should be revisited with the product owner before detailed implementation planning to confirm or expand scope.

## Technical hints (optional)

- New `TicketInternalComment` (or similar) entity in `src/BackEnd`, distinct from customer-facing communication (F03). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- @mentions, presence indicators, and team chat are not asserted as in scope (see Extra notes).
