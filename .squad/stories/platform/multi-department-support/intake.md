# Story intake

- Folder: `.squad/stories/platform/multi-department-support/intake.md`

---

## Feature

- **Feature name (display):** F12 — Platform
- **Feature slug (folder under `plans/`):** `platform`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-017`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F12, platform`

---

## Title

```
Multi-Department
```

---

## Description

```
Source Requirements: FR-063
Feature: F12 - Platform
Actor: Administrator (configuration); Agent/Supervisor (scoped access)

Business goal:
As an organization using the CRM, we want to operate multiple departments within
one CRM instance so that tickets, agents, and reporting can be organized/scoped by
department. Cross-Feature Requirement: "Multi-Department and Multi-Branch support
apply across the platform."

User Story: As an organization using the CRM, we want to operate multiple
departments within one CRM instance, so that tickets, agents, and reporting can be
organized/scoped by department.

Business value: Named cross-feature requirement, but the CRM is usable
single-department without it initially.

Priority: Should Have.

Business rules (from documentation):
- Multiple departments ... are supported (F12 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Administrator
When they create a department
Then the department is available for association with users/tickets

Given an authenticated Administrator
When they view or update the list of departments
Then the current department list/details are returned or updated

Given a non-Administrator
When they attempt to create or update departments
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none to introduce the Department concept itself; downstream stories (e.g. Ticket Assignment, Automatic Ticket Assignment, Reports) that later scope by department depend on this one.
- **Depends on code areas or other stories:** none.

## Extra notes (optional)

- Open question / assumption: the documentation states multi-department support applies "across the platform" but does not specify which entities (tickets, users, knowledge base articles, ...) carry a department association, nor whether department scoping restricts what a user can see. This story is scoped to establishing the Department entity and basic admin management; wiring department scoping into other features (tickets, reports, etc.) is called out as each of those stories' own responsibility, not duplicated here.

## Technical hints (optional)

- New entity/admin capability in `src/BackEnd` (Onion Architecture - Domain entity + repository + API), consistent with the existing generic repository pattern already scaffolded. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Multi-Branch (FR-064) - per the v1.4 revision note, this is a source-control/branching workflow clarification ("push user story implementations on GitHub on more than one branch"), not a product feature, and is not a Department-like data concept. No story was created for FR-064; see the project's requirements-coverage report.
