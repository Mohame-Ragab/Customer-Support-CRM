# Story intake

- Folder: `.squad/stories/platform/responsive-web-and-mobile-experience/intake.md`

---

## Feature

- **Feature name (display):** F12 — Platform
- **Feature slug (folder under `plans/`):** `platform`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-016`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F12, platform`

---

## Title

```
Web Friendly / Mobile Friendly
```

---

## Description

```
Source Requirements: FR-061, FR-062
Feature: F12 - Platform
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As a user on a desktop, tablet, or mobile device, I want the CRM's layout to work
well on my screen so that I can use it regardless of device.

User Story: As a user on a desktop, tablet, or mobile device, I want the CRM's
layout to work well on my screen, so that I can use it regardless of device.

Business value: Documented quality attribute; important but does not block any
single functional capability from shipping.

Priority: Should Have.

Business rules (from documentation):
- Web and mobile-friendly experiences are supported (F12 acceptance criteria).

Note on combining FR-061 and FR-062 into one story:
Both describe the same responsive-layout mechanism (one implementation, multiple
breakpoints) and share a single acceptance-criteria sentence in the source
documentation. Kept as one story for the same reason as the language pairing above.
```

---

## Acceptance criteria

```
Given the application layout foundation (AppLayout/AuthLayout and shared UI
  components)
When it is viewed on a desktop-width viewport
Then the full layout (e.g. persistent navigation) is usable

Given the same layout foundation
When it is viewed on a mobile-width viewport
Then it remains usable and legible (e.g. navigation collapses appropriately) without
  horizontal scrolling or unreadable/overlapping content

Given the RTL/LTR direction from "Multilingual Support"
When the viewport size changes
Then responsive behavior is correct in both directions
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** benefits from `platform/multilingual-support-arabic-and-english` being in place (RTL + responsive together).
- **Depends on code areas or other stories:** foundational; other stories' screens should follow the responsive patterns established here.

## Extra notes (optional)

- The documentation does not specify a native mobile app - "Mobile Friendly" is interpreted as a responsive web experience (a single React web app that adapts to mobile viewports), not a separate mobile application, since no native/mobile-app requirement or technology is mentioned anywhere in the documentation.

## Technical hints (optional)

- Frontend (`src/FrontEnd`) already uses MUI's flexbox-based layout primitives (`Box`, `Drawer`, `AppBar`), which are responsive by construction; this story is about establishing and verifying the concrete breakpoint behavior for the actual app shell. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- A native mobile application (not requested anywhere in the documentation).
