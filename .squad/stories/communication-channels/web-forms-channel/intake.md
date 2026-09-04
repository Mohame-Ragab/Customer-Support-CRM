# Story intake

- Folder: `.squad/stories/communication-channels/web-forms-channel/intake.md`

---

## Feature

- **Feature name (display):** F03 — Communication Channels
- **Feature slug (folder under `plans/`):** `communication-channels`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-034`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F03, communication-channels`

---

## Title

```
Web Forms Channel
```

---

## Description

```
Source Requirements: FR-025
Feature: F03 - Communication Channels
Actor: Customer (submitter)

Business goal:
As a Customer, I want to submit a support request through a web form so that a
ticket is created without needing to email or chat.

User Story: As a Customer, I want to submit a support request through a web form,
so that a ticket is created without needing to email or chat.

Business value: Cannot be confidently assessed independently of the overlap
question below - if this turns out to be the same capability as Customer Portal's
"Submit Tickets" (FR-041), building it separately would be duplicate work.

Priority: Needs Clarification - resolve the FR-025/FR-041 overlap (see Extra
notes) before this can be prioritized or estimated.

Business rules (from documentation):
- Web Forms can submit support requests (F03 acceptance criteria).
```

---

## Acceptance criteria

```
Given a visitor fills out the support web form with the required fields
When they submit it
Then a ticket is created from the submission (see "Create Ticket")

Given required fields are missing or invalid
When the form is submitted
Then it is rejected with a validation error
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `tickets/create-ticket`.
- **Depends on code areas or other stories:** overlaps with `customer-portal/submit-ticket-via-customer-portal` (F08, FR-041) - see Extra notes.

## Extra notes (optional)

- Open question: the documentation lists "Web Forms Channel" (F03/FR-025) and "Submit Tickets" via the Customer Portal (F08/FR-041) as two separate requirements with near-identical acceptance criteria ("Web Forms can submit support requests" vs. "Customer can submit tickets"). It is not clear from the documentation whether Web Forms means an unauthenticated, public intake form (e.g. embeddable on a marketing site, no login required) versus the authenticated Customer Portal's submission flow, or whether they are the same capability described twice. Both stories are kept (traceability to distinct FR IDs), but this overlap should be resolved with the product owner before implementation to avoid building the same capability twice.

## Technical hints (optional)

- If confirmed as a distinct, unauthenticated public form, this needs an anonymous-submission path into `tickets/create-ticket` (with appropriate anti-abuse/validation) rather than reusing the authenticated Customer Portal flow as-is. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Resolving the exact relationship to "Submit Ticket via Customer Portal" is explicitly called out above as needing product clarification, not decided in this story.
