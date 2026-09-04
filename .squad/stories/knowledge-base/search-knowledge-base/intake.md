# Story intake

- Folder: `.squad/stories/knowledge-base/search-knowledge-base/intake.md`

---

## Feature

- **Feature name (display):** F06 — Knowledge Base
- **Feature slug (folder under `plans/`):** `knowledge-base`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-046`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F06, knowledge-base`

---

## Title

```
Search
```

---

## Description

```
Source Requirements: FR-040
Feature: F06 - Knowledge Base
Actor: Agent, Customer (per F06's purpose: "for agents and customers")

Business goal:
As an Agent or Customer, I want to search the knowledge base so that I can quickly
find relevant FAQs, articles, or guides.

User Story: As an Agent or Customer, I want to search the knowledge base, so that
I can quickly find relevant FAQs, articles, or guides.

Business value: The primary way the content from "Manage Knowledge Base Content"
delivers value.

Priority: Should Have.

Business rules (from documentation):
- Knowledge Base can be searched (F06 acceptance criteria).
```

---

## Acceptance criteria

```
Given knowledge-base content exists (see "Manage Knowledge Base Content")
When a user searches with a keyword/phrase
Then matching content items (by title/body) are returned, ranked by relevance

Given a search with no matching results
When it is executed
Then an empty result set is returned (not an error)

Given the request is in Arabic or English
When a search is performed
Then search works correctly against content in that language
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `knowledge-base/manage-knowledge-base-content`.
- **Depends on code areas or other stories:** consumed by `customer-portal/access-faqs`.

## Extra notes (optional)

- Open question: the documentation does not specify search technology (simple SQL `LIKE`/full-text search vs. a dedicated search engine) - a technical decision to make during planning, not a business requirement.

## Technical hints (optional)

- Can start as a simple query over `KnowledgeBaseContent` (EF Core) in `src/BackEnd`; revisit for a dedicated search engine only if relevance/scale requires it (not indicated by the documentation). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Authoring/management of content (see "Manage Knowledge Base Content").
