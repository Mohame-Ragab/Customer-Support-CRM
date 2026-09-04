# Story intake

- Folder: `.squad/stories/customer-portal/access-faqs/intake.md`

---

## Feature

- **Feature name (display):** F08 — Customer Portal
- **Feature slug (folder under `plans/`):** `customer-portal`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-050`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F08, customer-portal`

---

## Title

```
Access FAQs
```

---

## Description

```
Source Requirements: FR-044
Feature: F08 - Customer Portal
Actor: Customer

Business goal:
As a Customer, I want to browse/search FAQs and knowledge-base content from the
portal so that I can self-serve before or instead of submitting a ticket.

User Story: As a Customer, I want to browse/search FAQs and knowledge-base
content from the portal, so that I can self-serve before or instead of
submitting a ticket.

Business value: Can reduce ticket volume, but depends entirely on Knowledge Base
content existing first and is not required for the Portal's core submit/track
loop.

Priority: Could Have.

Business rules (from documentation):
- Customer can ... access FAQs (F08 acceptance criteria).
```

---

## Acceptance criteria

```
Given knowledge-base content exists (see "Manage Knowledge Base Content")
When an authenticated Customer browses or searches FAQs from the portal
Then matching published content is returned (reusing "Search Knowledge Base")

Given a search with no matching results
When it is executed
Then an empty result set is returned (not an error)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `knowledge-base/manage-knowledge-base-content` and `knowledge-base/search-knowledge-base`.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- Open question: whether all knowledge-base content types (FAQ/Article/Solution-or-Guide) are customer-visible, or only content explicitly marked as customer-facing (vs. agent-internal solutions), is not specified in the documentation. Assumed here that all content is visible to both Agents and Customers, per F06's purpose statement ("for agents and customers"), pending clarification if an internal/external content distinction is actually intended.

## Technical hints (optional)

- Customer-facing wrapper around `knowledge-base/search-knowledge-base`'s API. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Authoring content (see "Manage Knowledge Base Content").
