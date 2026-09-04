# Story intake

Fill this template for each story you want planned. Keep it copy-paste-friendly: the planner reads **this file and the files in `attachments/`**, nothing else.

- Folder: `.squad/stories/auth/user-login/intake.md`
- Binaries (screenshots, PDFs, exports): put them in `attachments/` next to this file and list them below.
- Do **not** rely on external links (tracker URLs, wiki, chat) — the planner cannot open them. Paste the content you want considered.

This is **not** an implementation prompt. It is the input to the plan-generation meta-prompt bundled with squad-kit (`generate-plan.md` in the installed package).

---

## Feature

- **Feature name (display):** F00 — Authentication & Registration
- **Feature slug (folder under `plans/`):** `auth`

## Tracker (metadata only)

- **Tracker type:** `none`
- **Work item id:** `STORY-001`
- **Work item type:** `User Story`
- **Status:** `Draft`
- **Assignee:** ``
- **Labels:** `F00, auth`

External tracker links are **not** followed by the planner. Keep the id for naming and traceability only.

---

## Title

```
User Login
```

---

## Description

```
Source Requirements: FR-001
Feature: F00 - Authentication & Registration (Classification: Recommended addition)
Actors: Customer, Agent, Supervisor, Administrator

Business goal:
As a registered user (Customer, Agent, Supervisor or Administrator), I want to log in
with my credentials so that I can securely access the parts of the CRM my role is
authorized to use.

User Story: As a registered user (Customer, Agent, Supervisor, or Administrator), I
want to log in with my credentials, so that I can securely access the parts of the
CRM my role authorizes.

Business value: Nothing else in the CRM is reachable without authentication - this
is the entry point every other capability depends on.

Priority: Must Have - foundational; blocks every authenticated capability in the
system.

Business rules (from documentation):
- Valid credentials allow login.
- Invalid credentials are rejected.
- Access after login must be secure (F00 purpose: "Provide secure access for
  Customers, Agents, Supervisors and Administrators").

Notes:
This story issues the session's access token (and refresh token, so the session can
be kept alive via the separate "Refresh Access Token" story). Per-role authorization
of individual screens/endpoints is enforced by whichever feature owns that
screen/endpoint, not by this story.
```

---

## Acceptance criteria

```
Given a registered, verified user with valid credentials
When they submit their email/username and password to the login endpoint
Then they are authenticated and receive an access token (and refresh token)
And the response does not include the password or any credential material

Given a user submits an incorrect password or unknown identifier
When they attempt to log in
Then the login is rejected with an authentication error
And no token is issued
And the specific reason (unknown user vs wrong password) is not distinguishable in
  the response, to avoid revealing which accounts exist

Given a Customer account that has not completed required email verification (see
  "Email Verification", FR-004)
When they attempt to log in
Then the system rejects or restricts the login per the "verification required"
  business rule from F00's acceptance criteria ("Customers can register and access
  the portal after required verification")

Given the request is in Arabic (Accept-Language: ar) or English (Accept-Language: en)
When a login error is returned
Then the error message is localized accordingly
```

---

## Attachments

Place files in `attachments/` next to this `intake.md`, then list them here so the planner knows what to open.

| File (relative to this folder) | What it is |
| ------------------------------ | ---------- |
| None. | — |

---

## Dependencies

- **Blocked by / related ids:** Depends on user accounts existing (created via "Customer Registration" for Customers, or via "Manage Users" for Agent/Supervisor/Administrator accounts).
- **Depends on code areas or other stories:** `auth/customer-registration`, `security-admin/manage-users`; pairs with `auth/refresh-access-token` and `auth/logout`.

## Extra notes (optional)

- Open question: the documentation does not specify whether login accepts email, username, or either. Default to email as the login identifier (ASP.NET Core Identity default) unless clarified.
- Open question: account lockout policy (failed-attempt threshold) is not specified in the documentation; not included in acceptance criteria to avoid inventing a business rule.

## Technical hints (optional)

- Backend already scaffolds JWT authentication middleware and ASP.NET Core Identity (Admin/Supervisor/Manager/Agent roles) in `src/BackEnd` (Onion Architecture) — this story is the first to actually issue a token via that pipeline. Frontend already has `AuthProvider.login(accessToken)` and `ProtectedRoute` waiting in `src/FrontEnd` (see each side's `docs/*-architecture.md`, "Next step"). Repos/roots: `.`. Primary language: `C# TypeScript`.

## Out of scope

- Registration, email verification, password reset/change, refresh-token issuance-on-expiry, and profile management are separate stories (see Dependencies).
- MFA is not mentioned in the documentation and is out of scope.
