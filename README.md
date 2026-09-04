# Customer Support CRM

A full-stack Customer Support CRM: ticket management, multi-channel communication
(email / live chat / web forms), a knowledge base, a self-service customer portal,
an agent dashboard, management reporting, and role-based administration —
built with an ASP.NET Core Web API backend and a React + TypeScript frontend.

## Architecture

**Backend** — ASP.NET Core Web API (.NET 10) on **Onion / Clean Architecture**:

- **Domain** — entities, enums, domain exceptions. No framework dependencies.
- **Application** — CQRS via **MediatR** (commands/queries + handlers), DTOs,
  **FluentValidation** validators, and the port interfaces (`IUnitOfWork`,
  `IGenericRepository<T>`, `ICurrentUserService`, …) the outer layers implement.
- **Infrastructure** — **Entity Framework Core** (SQL Server) implementing the
  generic repository/unit-of-work pattern, **ASP.NET Core Identity**, JWT
  issuing, email (MailKit/SMTP), and other external-facing services.
- **API** — controllers, JWT bearer authentication, Swagger/OpenAPI, global
  exception handling, localization middleware, SignalR (live chat).

Cross-cutting:
- **JWT authentication** with short-lived access tokens and **refresh-token
  rotation** (single-use, replay-detected, revocable) — the refresh token
  itself travels as an HttpOnly, SameSite cookie, never exposed to JavaScript.
- **Role-based authorization** (`Admin`, `Supervisor`, `Manager`, `Agent`,
  `Customer`) enforced on every protected endpoint.
- **English / Arabic localization** (RTL-aware) on both backend (`Accept-Language`
  → `SharedResource` resx) and frontend (i18next).

**Frontend** — **React 19 + TypeScript**, built with Vite:

- **MUI** components, **TanStack Query** for server state, **React Hook Form**
  for forms, **React Router** for routing/role-guarded routes.
- Feature-sliced structure (`src/features/<feature>`), a single shared
  Axios `apiClient` with request/response interceptors (auth header,
  language header, silent token refresh).
- **i18next** (English/Arabic) with automatic RTL layout switching.
- **SignalR** client for the live chat channel.

## Implemented Features

- **Authentication & Identity** — registration, login, JWT access + refresh
  tokens (rotation, revocation), logout, password reset/change, email
  verification, seeded initial Admin account, Customer ↔ portal-account linking.
- **Customer Management** — CRUD, notes, attachments, interaction history.
- **Ticket Management** — creation, classification (category/priority),
  assignment, status lifecycle, escalation, full audit history.
- **Communication Channels** — inbound/outbound ticket email, live chat
  (SignalR), public web-form ticket intake.
- **Agent Dashboard** — personal task/reminder list, assigned-tickets view,
  internal (staff-only) ticket comments, quick-reply templates.
- **Knowledge Base** — article authoring/publishing, full-text search.
- **Customer Portal** — self-service ticket submission and tracking, FAQ
  browsing, post-resolution satisfaction feedback.
- **Reports & Management** — ticket volume/status/category/priority reports,
  agent performance reports, customer satisfaction reports, a management
  dashboard summarizing all three.
- **Security & Administration** — user management, role/permission
  management, audit logs, system configuration.
- **Platform** — multi-department support, custom branding, responsive layout,
  full English/Arabic localization.

> **FR-047 (SLA Performance) was intentionally removed from scope** (it
> depended on a deleted `sla-automation` prerequisite) and is not implemented.

## Project Structure

```
CustomerSupportCRM/
├── src/
│   ├── BackEnd/
│   │   ├── CustomerSupportCRM.sln
│   │   ├── docs/architecture.md          # backend architecture reference
│   │   └── src/
│   │       ├── CustomerSupportCRM.Domain
│   │       ├── CustomerSupportCRM.Application
│   │       ├── CustomerSupportCRM.Infrastructure
│   │       └── CustomerSupportCRM.API
│   └── FrontEnd/
│       ├── docs/frontend-architecture.md # frontend architecture reference
│       └── src/
│           ├── features/                 # one folder per feature slice
│           ├── pages/, routes/, layouts/
│           ├── lib/ (api client, auth, i18n, query)
│           └── components/ui             # shared presentational components
└── .squad/                               # planning artifacts (stories/plans), not application code
```

## Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20+ and npm
- SQL Server (LocalDB, Developer, or Express edition) reachable from your
  machine

## Backend Setup

```bash
cd src/BackEnd
dotnet restore
dotnet build CustomerSupportCRM.sln
```

Configure the database connection and JWT secret first (see
[Configuration](#configuration) below), then run the API:

```bash
cd src/CustomerSupportCRM.API
dotnet run
```

By default this listens on `http://localhost:5025` and
`https://localhost:7125` (see `Properties/launchSettings.json`).

## Database Setup

The backend targets **SQL Server** via EF Core. Set your connection string in
`appsettings.Development.json` (or an environment variable — see
[Configuration](#configuration)), then apply migrations from the API project
directory:

```bash
cd src/BackEnd/src/CustomerSupportCRM.API
dotnet ef database update --project ../CustomerSupportCRM.Infrastructure --startup-project .
```

This creates the schema and applies all migrations (Identity tables, tickets,
customers, communication channels, agent-dashboard, knowledge base, customer
feedback, and the Customer↔ApplicationUser link). On startup, the API also
seeds:
- the standard roles (`Admin`, `Supervisor`, `Manager`, `Agent`, `Customer`),
- the `Admin` role's default permission set, and
- an initial Admin account, **if** `AdminSeed:Email` / `AdminSeed:Password`
  are configured (see below) — idempotent, safe to run repeatedly.

## Frontend Setup

```bash
cd src/FrontEnd
npm install
cp .env.example .env.local   # then adjust VITE_API_BASE_URL if needed
npm run dev
```

Available scripts (`package.json`):

| Command                                   | Purpose                                              |
| ------------------------------------------ | ----------------------------------------------------- |
| `npm run dev`                              | Start the Vite dev server                             |
| `npm run build`                            | Type-check (`tsc -b`) and produce a production build |
| `npm run preview`                          | Serve the production build locally                    |
| `npm run lint`                             | ESLint                                                |
| `npm run format` / `npm run format:check`  | Prettier                                              |

## Authentication

- **Register** (`POST /api/v1/auth/register`) — self-service sign-up; creates
  an `ApplicationUser` in the `Customer` role and a linked `Customer` CRM
  record. Staff accounts (`Admin`/`Supervisor`/`Manager`/`Agent`) are created
  by an Administrator through user management, not self-registration.
- **Login** (`POST /api/v1/auth/login`) — returns a short-lived JWT access
  token; the refresh token is set as an HttpOnly cookie.
- **Admin access** — an initial Admin account is seeded on first run from the
  `AdminSeed` configuration (see [Configuration](#configuration)).
- **Roles** — `Admin`, `Supervisor`, `Manager`, `Agent`, `Customer`; enforced
  via `[Authorize(Roles = ...)]` on every protected endpoint.
- **Access token** — short-lived JWT, sent as `Authorization: Bearer <token>`.
- **Refresh token** — long-lived, single-use, rotated on every refresh,
  revocable, transported only via an HttpOnly/SameSite cookie
  (`POST /api/v1/auth/refresh`).
- **Logout** (`POST /api/v1/auth/logout`) — revokes the refresh token
  server-side and clears the cookie.

## Localization

- **English** and **Arabic** are fully supported end to end: backend
  validation/error messages (`Accept-Language` → `SharedResource`/`.ar.resx`)
  and frontend UI copy (i18next, `src/FrontEnd/src/lib/i18n/resources/{en,ar}`).
- The frontend automatically switches between **LTR** (English) and **RTL**
  (Arabic) layout based on the selected language.

## Swagger

With the API running in the `Development` environment, open:

```
https://localhost:7125/swagger
```

for interactive API documentation, including the JWT Bearer auth scheme (use
the "Authorize" button with a token obtained from `/api/v1/auth/login`).

## Configuration

All backend configuration lives in `src/BackEnd/src/CustomerSupportCRM.API/appsettings*.json`,
overridable by environment variables (`Section__Key=value`) or `dotnet user-secrets`
for local development. **No real secrets are committed** — every sensitive
value ships empty in `appsettings.json`/`appsettings.Production.json`, or as
an obviously-fake, clearly-labeled placeholder in `appsettings.Development.json`
(safe for local development only — replace it for anything beyond your own
machine).

| Setting | Where | Purpose |
| --- | --- | --- |
| `ConnectionStrings:DefaultConnection` | `appsettings.json` | SQL Server connection string. Ships with Windows Integrated Security against a local `.` instance — replace with your own server/credentials. |
| `Jwt:SecretKey` | `appsettings.Development.json` (dev placeholder) / env var in other environments | JWT signing key. **Must** be set to a strong, private value outside local development. |
| `AdminSeed:Email` / `AdminSeed:Password` | `appsettings.Development.json` (dev placeholder) / env var (`AdminSeed__Email` / `AdminSeed__Password`) | Seeds one initial Admin account on startup. Empty by default (`appsettings.json`) — seeding is skipped entirely if unset. |
| `Email:*` | `appsettings.Development.json` / env vars | SMTP settings for the email communication channel (optional locally). |
| `VITE_API_BASE_URL` | `src/FrontEnd/.env.local` (copy from `.env.example`) | Base URL the frontend uses to reach the backend API. |

## Documentation

- Backend architecture reference: [`src/BackEnd/docs/architecture.md`](src/BackEnd/docs/architecture.md)
- Frontend architecture reference: [`src/FrontEnd/docs/frontend-architecture.md`](src/FrontEnd/docs/frontend-architecture.md)
#   C u s t o m e r - S u p p o r t - C R M  
 