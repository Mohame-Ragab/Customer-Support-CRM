# Story intake

- Folder: `.squad/stories/platform/multilingual-support-arabic-and-english/intake.md`

---

## Feature

- **Feature name (display):** F12 — Platform
- **Feature slug (folder under `plans/`):** `platform`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-015`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F12, platform`

---

## Title

```
Arabic Language / English Language
```

---

## Description

```
Source Requirements: FR-059, FR-060
Feature: F12 - Platform
Actors: Customer, Agent, Supervisor, Administrator (all users of the platform)

Business goal:
As a user of any role, I want to use the CRM in Arabic or English so that I can
work in my preferred/organization's language.

User Story: As a user of any role, I want to use the CRM in Arabic or English, so
that I can work in my preferred/organization's language.

Business value: Explicitly named cross-feature requirement affecting the entire
platform and user-facing features; core to the product's stated purpose.

Priority: Must Have.

Business rules (from documentation):
- Arabic and English are supported (F12 acceptance criteria).
- Cross-Feature Requirement: Localization - Arabic and English affect the platform
  and user-facing features.

Note on combining FR-059 and FR-060 into one story:
Both requirements describe the same underlying localization mechanism (one
implementation that supports both languages simultaneously) and share a single
acceptance-criteria sentence in the source documentation ("Arabic and English are
supported."). Splitting them into two stories would duplicate the same
implementation rather than represent two independent capabilities, which the task's
own rules ask to avoid.
```

---

## Acceptance criteria

```
Given a user selects Arabic
When any system-provided label, message, or validation error is displayed
Then it is shown in Arabic, and the layout switches to right-to-left (RTL)

Given a user selects English
When any system-provided label, message, or validation error is displayed
Then it is shown in English, in left-to-right (LTR) layout

Given no explicit language selection has been made yet
When the user first loads the application
Then English (en) is used as the default/fallback language

Given a backend API request is made
When the client sends an Accept-Language header of "ar" or "en"
Then backend-generated messages (validation errors, ProblemDetails) are returned in
  the matching language
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none - this is foundational platform infrastructure (Recommended Implementation Sequence: Phase 2, "F12 Platform foundation").
- **Depends on code areas or other stories:** every other story with user-facing text depends on this one for its localized strings; those stories should add their own translation keys/resource entries rather than hardcoding text, but do not need to re-implement localization.

## Extra notes (optional)

- None beyond the combination rationale above.

## Technical hints (optional)

- Already scaffolded end to end: backend `RequestLocalizationMiddleware` + `IStringLocalizer<SharedResource>` + `SharedResource.resx`/`SharedResource.ar.resx` in `src/BackEnd`; frontend `i18next`/`react-i18next` + RTL-aware MUI theme (`stylis-plugin-rtl`) + `useDirection()` in `src/FrontEnd`. This story is about *using* that infrastructure for the CRM's real feature strings as each feature ships, and confirming the default/fallback behavior end to end - not building the mechanism from scratch. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Feature-specific translation keys are added by each feature's own story, not enumerated here.
