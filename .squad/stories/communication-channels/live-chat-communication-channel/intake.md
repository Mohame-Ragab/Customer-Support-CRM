# Story intake

- Folder: `.squad/stories/communication-channels/live-chat-communication-channel/intake.md`

---

## Feature

- **Feature name (display):** F03 — Communication Channels
- **Feature slug (folder under `plans/`):** `communication-channels`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-033`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F03, communication-channels`

---

## Title

```
Live Chat Channel
```

---

## Description

```
Source Requirements: FR-024
Feature: F03 - Communication Channels
Actor: Customer, Agent

Business goal:
As a Customer, I want to chat live with an Agent so that I can get real-time
support. As an Agent, I want to respond to live chats so that I can help
customers in real time.

User Story: As a Customer, I want to chat live with an Agent, so that I can get
real-time support. As an Agent, I want to respond to live chats, so that I can
help customers in real time.

Business value: Valuable real-time channel, but the CRM can function on email
alone initially.

Priority: Should Have.

Business rules (from documentation):
- Live Chat ... is supported (F03 acceptance criteria).
```

---

## Acceptance criteria

```
Given a Customer starts a live chat session
When they send a message
Then an available Agent can see and respond to it in real time

Given a live chat session is tied to a ticket (or creates one)
When the conversation happens
Then the messages are recorded and associated with that ticket

Given no Agent is currently available
When a Customer starts a chat
Then the system handles it gracefully (documentation does not specify the exact
  fallback behavior - flagged as an open question rather than invented)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** relates to `tickets/create-ticket` (a chat may create or attach to a ticket).
- **Depends on code areas or other stories:** contributes data to `customers/view-customer-interaction-history`.

## Extra notes (optional)

- Open question: real-time transport mechanism (e.g. WebSockets/SignalR) is a technical decision not specified in the documentation.
- Open question: whether every chat becomes a ticket, or chat is a separate, lighter-weight interaction type, is not specified.
- Open question: Agent-unavailable fallback behavior (queue? offline message? no chat initiation allowed?) is not specified.

## Technical hints (optional)

- No real-time communication infrastructure exists yet in `src/BackEnd`/`src/FrontEnd`; this is new scope (e.g. SignalR on the backend). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- WhatsApp and SMS channels (explicitly removed from scope per the v1.4 revision).
