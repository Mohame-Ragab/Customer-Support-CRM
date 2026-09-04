# Story intake

- Folder: `.squad/stories/auth/refresh-access-token/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-008`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Refresh Token
```

---

## Description

```
Source Requirements: FR-008
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As a logged-in user, I want my session to be extended via a refresh token so that
I am not forced to re-enter my credentials every time my short-lived access token
expires.

User Story: As a logged-in user, I want my session to be extended via a refresh
token, so that I am not forced to re-enter my credentials every time my short-lived
access token expires.

Business value: Without this, short-lived access tokens force disruptively frequent
re-logins - directly affects the usability of every other authenticated story.

Priority: Must Have - required for a workable session length once access tokens
expire.

Business rules (from documentation):
- Not elaborated beyond the FR-008 title "Refresh Token"; behavior derived from the
  standard JWT access/refresh token pattern already anticipated by the existing
  backend/frontend architecture (see Technical hints).
```

---

## Acceptance criteria

```
Given a user holds a valid, unexpired refresh token issued at login
When they submit it to the refresh endpoint
Then a new access token (and, per rotation policy, a new refresh token) is issued
And the user's session continues without re-entering credentials

Given a refresh token that is expired, already used (if rotation invalidates prior
  tokens), or unknown/revoked
When it is submitted
Then the refresh is rejected as unauthorized, and the client is treated as logged
  out (see "Logout")
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** depends on `auth/user-login` (refresh token is issued there).
- **Depends on code areas or other stories:** interacts with `auth/logout`'s revocation behavior.

## Extra notes (optional)

- Open question: whether refresh tokens rotate (single-use, reissued each refresh) or are long-lived and reusable is not specified in the documentation; rotation is the safer default and is assumed unless clarified.
- Open question: refresh token lifetime/expiry is not specified.

## Technical hints (optional)

- Backend's `src/BackEnd` JWT setup currently issues only access tokens (`Jwt:ExpirationMinutes`); this story adds the refresh-token issuance/storage/rotation and the `/refresh` endpoint. Frontend's `lib/auth/tokenStorage.ts` (`src/FrontEnd`) currently documents an in-memory-only access token with a target architecture of an HttpOnly refresh cookie — this story is what makes that real (see `src/FrontEnd/docs/frontend-architecture.md`, "Token storage"). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Where the refresh token is stored on the client is a frontend architecture decision already documented in `src/FrontEnd/docs/frontend-architecture.md` (§7) — this story implements the backend contract that decision depends on, not the storage decision itself.
