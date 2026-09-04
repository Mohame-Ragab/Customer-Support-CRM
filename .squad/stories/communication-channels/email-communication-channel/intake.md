# Story intake

- Folder: `.squad/stories/communication-channels/email-communication-channel/intake.md`

---

## Feature

- **Feature name (display):** F03 — Communication Channels
- **Feature slug (folder under `plans/`):** `communication-channels`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-032`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F03, communication-channels`

---

## Title

```
Email Channel
```

---

## Description

```
Source Requirements: FR-023
Feature: F03 - Communication Channels (Classification: Source-derived, revised -
  WhatsApp and SMS channels have been removed from scope per this revision)
Actor: Customer (sends/receives), Agent (sends/receives)

Business goal:
As a Customer or Agent, I want ticket-related communication to happen over email so
that a conversation tied to a ticket can be carried on through the customer's inbox.

User Story: As a Customer or Agent, I want ticket-related communication to happen
over email, so that a conversation tied to a ticket can be carried on through the
customer's inbox.

Business value: Explicitly the first-listed retained channel in the
documentation; the most universally expected support channel.

Priority: Must Have.

Business rules (from documentation):
- Email ... is supported (F03 acceptance criteria).
- WhatsApp and SMS channels are explicitly out of scope (Removed section of F03).
```

---

## Acceptance criteria

```
Given a ticket exists
When an Agent sends an email reply on that ticket
Then the message is sent to the customer's email and recorded against the ticket

Given a customer replies by email to a ticket-related message
When the reply is received
Then it is associated with the correct ticket and visible to staff as part of that
  ticket's communication

Given an email fails to send (e.g. provider error)
When the failure occurs
Then it is surfaced to the sending Agent rather than silently lost
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** contributes data to `customers/view-customer-interaction-history`; may share transactional-email infrastructure with `auth/email-verification` and `auth/forgot-password` at a technical level (see that story's note), though those are triggered by the system, not by staff/customer conversation.

## Extra notes (optional)

- Open question: the specific email provider/protocol (SMTP relay, a transactional email API, inbound parsing for replies) is not specified in the documentation; this is a technical decision to make during planning, not asserted here. No third-party integration is named in the documentation (F11 Integrations was explicitly removed from scope), so this should be implemented as a basic, self-contained email capability, not as a specific vendor integration.

## Technical hints (optional)

- No email-sending infrastructure exists yet in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- WhatsApp and SMS channels (explicitly removed from scope per the v1.4 revision).
- Third-party integrations (F11, removed from scope).
