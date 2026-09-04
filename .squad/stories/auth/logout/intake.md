# Story intake

- Folder: `.squad/stories/auth/logout/intake.md`

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-003`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

---

## Title

```
Logout
```

---

## Description

```
Source Requirements: FR-003
Feature: F00 - Authentication & Registration
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As an authenticated user of any role, I want to log out so that my session is
ended and my credentials/tokens can no longer be used from this client.

User Story: As an authenticated user of any role, I want to log out, so that my
session is ended and my credentials/tokens can no longer be used from this client.

Business value: Basic session-hygiene expectation for a secure system.

Priority: Must Have - explicit, named F00 acceptance criterion; a security/UX
baseline.

Business rules (from documentation):
- Users can logout (F00 acceptance criteria).
```

---

## Acceptance criteria

```
Given an authenticated user with a valid session
When they request logout
Then their current tokens are invalidated on the client (and, if a server-side
  refresh-token store exists, the refresh token is revoked there too)
And subsequent requests using the old access token after its natural expiry are
  rejected as unauthorized

Given a logout request is made without a valid/active session
When it is submitted
Then it is handled gracefully (no error that leaks session internals)
```

---

## Attachments

| File | What it is |
| ---- | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** requires `auth/user-login` to exist first (nothing to log out of otherwise).
- **Depends on code areas or other stories:** shares the token/session model with `auth/refresh-access-token`.

## Extra notes (optional)

- Open question: whether refresh tokens are tracked server-side (revocable) or are stateless is not specified in the documentation; the frontend's existing token-storage design (`src/FrontEnd/docs/frontend-architecture.md`, "Token storage") keeps the access token in memory only, so a client-side logout already clears it - server-side revocation of a persisted refresh token depends on how "Refresh Token" (FR-008) is ultimately implemented.

## Technical hints (optional)

- Frontend already has `AuthProvider.logout()` and `lib/auth/tokenStorage.ts` (`src/FrontEnd`) ready to be called; this story wires a real trigger (button + optional backend call) to it. Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Global "log out of all devices" is not mentioned in the documentation.
