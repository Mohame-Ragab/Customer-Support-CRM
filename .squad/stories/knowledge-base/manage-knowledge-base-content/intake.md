# Story intake

- Folder: `.squad/stories/knowledge-base/manage-knowledge-base-content/intake.md`

---

## Feature

- **Feature name (display):** F06 — Knowledge Base
- **Feature slug (folder under `plans/`):** `knowledge-base`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-045`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F06, knowledge-base`

---

## Title

```
FAQs / Help Articles / Solutions and Guides
```

---

## Description

```
Source Requirements: FR-037, FR-038, FR-039
Feature: F06 - Knowledge Base (Classification: Source-derived)
Actor: Agent, Supervisor, Administrator (authoring); Agent, Customer (consuming -
  see "Search Knowledge Base" and "Access FAQs")

Business goal:
As staff, I want to create and maintain FAQs, help articles, and solutions/guides
so that agents and customers have searchable, self-service support content.

User Story: As staff, I want to create and maintain FAQs, help articles, and
solutions/guides, so that agents and customers have searchable, self-service
support content.

Business value: Foundational - nothing in F06/F08's FAQ-related stories has
content to show without this.

Priority: Should Have.

Business rules (from documentation):
- FAQs, articles, solutions and guides can be managed (F06 acceptance criteria - a
  single sentence covering FR-037, FR-038, and FR-039, which is why they are
  combined into one story here rather than duplicated across three near-identical
  content-management stories).

Note on combining FR-037/038/039:
Each is a piece of textual knowledge-base content differing only by a type label
(FAQ / Article / Solution-or-Guide), managed through the same create/view/update
mechanism. Modeled as one "content item" concept with a type attribute.
```

---

## Acceptance criteria

```
Given authenticated staff (Agent, Supervisor, or Administrator)
When they create a knowledge-base content item (title, body, and type: FAQ /
  Article / Solution-or-Guide)
Then it is saved and becomes available for search/browsing (see "Search Knowledge
  Base")

Given authenticated staff
When they view or update an existing content item
Then the current/updated content is returned

Given a content item is missing required fields (e.g. title or body)
When it is created or updated
Then it is rejected with a validation error

Given the request is in Arabic or English
When content is authored
Then the content itself can be authored/stored per-language (documentation does not
  specify bilingual content authoring in detail; flagged as an open question rather
  than assumed)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** feeds `knowledge-base/search-knowledge-base` and `customer-portal/access-faqs`.

## Extra notes (optional)

- Open question: whether each content item needs a separate Arabic and English version (bilingual authoring) or the UI-level localization (Multilingual Support story) is sufficient for the surrounding chrome while content itself is single-language, is not specified in the documentation.
- Open question: whether Customers can author/suggest content, or only staff - the documentation lists this feature's purpose as "for agents and customers" in the sense of *access*, not authoring; authoring is scoped to staff here.

## Technical hints (optional)

- New `KnowledgeBaseContent` entity with a `ContentType` (FAQ/Article/Solution) in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Search (see "Search Knowledge Base").
- Customer-facing access surface (see "Access FAQs" in the Customer Portal).
