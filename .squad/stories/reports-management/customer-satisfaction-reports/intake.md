# Story intake

- Folder: `.squad/stories/reports-management/customer-satisfaction-reports/intake.md`

---

## Feature

- **Feature name (display):** F09 — Reports & Management
- **Feature slug (folder under `plans/`):** `reports-management`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-055`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F09, reports-management`

---

## Title

```
Customer Satisfaction
```

---

## Description

```
Source Requirements: FR-049
Feature: F09 - Reports & Management
Actor: Supervisor, Administrator

Business goal:
As a Supervisor/Administrator, I want to see aggregated customer satisfaction so
that I can measure how well support is performing from the customer's perspective.

User Story: As a Supervisor/Administrator, I want to see aggregated customer
satisfaction, so that I can measure how well support is performing from the
customer's perspective.

Business value: Explicitly named in F09's purpose; entirely dependent on "Submit
Customer Feedback"'s data.

Priority: Should Have.

Business rules (from documentation):
- Customer satisfaction is reportable (F09 acceptance criteria).
```

---

## Acceptance criteria

```
Given customer feedback has been submitted (see "Submit Customer Feedback")
When an authorized user requests a customer satisfaction report for a period
Then aggregated satisfaction metrics (derived from the feedback structure defined
  in "Submit Customer Feedback") are returned

Given no feedback has been submitted yet for the requested period
When the report is requested
Then it returns an empty/zero result, not an error

Given a Customer or Agent
When they attempt to access customer satisfaction reports
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `customer-portal/submit-customer-feedback` - this report's exact shape depends on that story's feedback-structure decision (see that story's open question).
- **Depends on code areas or other stories:** feeds `reports-management/management-dashboards`.

## Extra notes (optional)

- This story cannot be fully specified until the feedback structure (rating scale vs. free text vs. both) is confirmed in "Submit Customer Feedback."

## Technical hints (optional)

- Aggregation over `CustomerFeedback` in `src/BackEnd`. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- The feedback-collection mechanism itself (see "Submit Customer Feedback").
