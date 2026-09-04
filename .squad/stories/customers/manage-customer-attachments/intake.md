# Story intake

- Folder: `.squad/stories/customers/manage-customer-attachments/intake.md`

---

## Feature

- **Feature name (display):** F01 — Customer Management
- **Feature slug (folder under `plans/`):** `customers`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-024`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F01, customers`

---

## Title

```
Customer Attachments
```

---

## Description

```
Source Requirements: FR-014
Feature: F01 - Customer Management
Actor: Agent, Supervisor, Administrator

Business goal:
As staff, I want to upload and view files attached to a customer's record so that
relevant documents (contracts, screenshots, etc.) are kept with the customer.

User Story: As staff, I want to upload and view files attached to a customer's
record, so that relevant documents are kept with the customer.

Business value: Useful, but the least essential of F01's capabilities for a
minimum viable support workflow.

Priority: Could Have.

Business rules (from documentation):
- Attachments can be managed (F01 acceptance criteria).
```

---

## Acceptance criteria

```
Given authenticated staff and an existing customer
When they upload a file as an attachment
Then the file is stored and associated with that customer, with the uploader and
  timestamp recorded

Given authenticated staff and an existing customer
When they view/list that customer's attachments
Then the attachments are returned with enough metadata to identify and download
  each one

Given a Customer (self-service) or unauthenticated user
When they attempt to upload or view attachments through this staff-facing
  capability
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customers/create-customer`.
- **Depends on code areas or other stories:** none further.

## Extra notes (optional)

- Open question: the documentation does not specify allowed file types/size limits, storage location (local vs. cloud object storage), nor whether attachments can be deleted. These are product/technical decisions to make before implementation, not assumed here.

## Technical hints (optional)

- No file-storage mechanism exists yet in `src/BackEnd`; this story introduces it (needs a storage decision - e.g. local disk vs. cloud blob storage - the documentation does not specify one). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Deletion of attachments (see Extra notes - open question).
