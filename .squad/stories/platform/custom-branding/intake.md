# Story intake

- Folder: `.squad/stories/platform/custom-branding/intake.md`

---

## Feature

- **Feature name (display):** F12 — Platform
- **Feature slug (folder under `plans/`):** `platform`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-018`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F12, platform`

---

## Title

```
Custom Branding
```

---

## Description

```
Source Requirements: FR-065
Feature: F12 - Platform
Actor: Administrator (configuration); all users (see the applied branding)

Business goal:
As an organization deploying the CRM, we want to apply our own branding (at least
logo and brand colors, the minimum implied by "Custom Branding") so that the CRM
reflects our organization's identity.

User Story: As an organization deploying the CRM, we want to apply our own
branding, so that the CRM reflects our organization's identity.

Business value: Cosmetic; no functional capability depends on it.

Priority: Could Have.

Business rules (from documentation):
- Custom branding is supported (F12 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated Administrator
When they set a custom logo and/or brand colors
Then those changes are persisted

Given branding has been customized
When any user loads the application
Then the customized logo/colors are applied throughout the UI (theme)

Given a non-Administrator
When they attempt to change branding settings
Then the request is forbidden
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** none.
- **Depends on code areas or other stories:** builds on the design-system/theme foundation (`styles/theme/`) already present in `src/FrontEnd`.

## Extra notes (optional)

- Open question / assumption: the documentation does not enumerate exactly which branding elements are customizable (logo, primary/secondary colors, favicon, application name?). Scoped to the minimum reasonable set (logo + brand color) pending product clarification; may relate to "Manage System Configuration" (F10) as the storage mechanism rather than being a fully separate subsystem - flagged as a possible overlap to resolve during planning.

## Technical hints (optional)

- Frontend theme foundation (`styles/theme/createAppTheme.ts`, `palette.ts`) in `src/FrontEnd` is the natural extension point for admin-supplied brand colors; the logo/asset needs a storage location (new scope). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- General system configuration unrelated to branding (see "Manage System Configuration").
