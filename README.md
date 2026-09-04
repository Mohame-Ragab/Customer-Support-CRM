# Customer Support CRM

A full-stack Customer Support CRM for managing tickets, customers, and
multi-channel communication — built with an **ASP.NET Core Web API** backend
(Onion Architecture, CQRS/MediatR) and a **React + TypeScript** frontend.

It covers the full support workflow: ticket intake and lifecycle management,
a self-service customer portal, an agent dashboard, a knowledge base,
management reporting, and role-based administration — with full
English/Arabic localization.

---

## 📋 Overview

Customer Support CRM lets a support organization manage customer
relationships and tickets end to end:

- Customers submit and track tickets through a self-service portal.
- Agents work tickets from a dedicated dashboard with internal notes and
  quick replies.
- Managers monitor performance through reports and a management dashboard.
- Administrators control roles, permissions, and system configuration.

---

## ✨ Features

### Authentication & Identity

- Self-service registration
- Login with JWT access tokens
- Refresh-token rotation (single-use, revocable)
- Logout with server-side token revocation
- Password reset and change
- Email verification
- Role-based authorization
- Seeded initial Admin account

### Customer Management

- Customer CRUD
- Customer notes
- Customer attachments
- Customer interaction history
- Linking between a Customer record and its portal account

### Ticket Management

- Ticket creation and classification (category, priority)
- Agent assignment
- Status lifecycle management
- Escalation
- Full ticket audit history

### Communication

- Inbound/outbound ticket email
- Live chat (SignalR)
- Public web-form ticket intake

### Agent Dashboard

- Assigned-tickets view
- Personal tasks and reminders
- Internal (staff-only) ticket comments
- Quick-reply templates
- Customer information at a glance

### Knowledge Base

- Article authoring and publishing
- Full-text search

### Customer Portal

- Self-service ticket submission
- Ticket tracking and history
- FAQ browsing
- Post-resolution satisfaction feedback

### Reports & Management

- Ticket volume, status, category, and priority reports
- Agent performance reports
- Customer satisfaction reports
- Management dashboard summarizing all reports

### Security & Administration

- User management
- Role and permission management
- Audit logs
- System configuration

### Platform

- Multi-department support
- Custom branding
- Responsive layout
- Full English/Arabic localization (RTL/LTR)
- Swagger/OpenAPI documentation

> **Note:** SLA Performance tracking was intentionally excluded from scope
> and is not implemented.

---

## 🏗️ Architecture

The backend follows **Onion (Clean) Architecture**:

| Layer          | Responsibility                                                          |
| -------------- | ------------------------------------------------------------------------ |
| Domain         | Entities, enums, domain exceptions — no framework dependencies          |
| Application    | CQRS via MediatR, DTOs, FluentValidation, port interfaces                |
| Infrastructure | EF Core, ASP.NET Core Identity, generic repository/unit of work, email  |
| API            | Controllers, JWT auth, Swagger, global exception handling, localization |

Key patterns:

- **CQRS / MediatR** — commands and queries with dedicated handlers.
- **Generic Repository + Unit of Work** — `IGenericRepository<T>` /
  `IUnitOfWork` abstract data access behind the Application layer.
- **ASP.NET Core Identity** — user accounts, roles, and password hashing.
- **JWT authentication** — short-lived access tokens.
- **Refresh-token rotation** — single-use, replay-detected refresh tokens
  transported via an HttpOnly cookie.

The frontend is a **feature-sliced React application**: each feature owns
its API calls, hooks, and components, backed by a shared Axios client and
TanStack Query for server state.

---

## 🛠️ Technology Stack

| Area               | Technology                    |
| ------------------ | ------------------------------ |
| Backend            | ASP.NET Core Web API           |
| Framework          | .NET 10                        |
| Database           | SQL Server                     |
| ORM                | Entity Framework Core          |
| Authentication     | ASP.NET Core Identity + JWT    |
| Validation         | FluentValidation               |
| Object Mapping     | AutoMapper                     |
| Real-time          | SignalR                        |
| Email              | MailKit                        |
| API Documentation  | Swagger / OpenAPI              |
| Frontend           | React 19 + TypeScript          |
| Build Tool         | Vite                           |
| State Management   | TanStack Query                 |
| Forms              | React Hook Form                |
| Routing            | React Router                   |
| UI Components      | Material UI (MUI)               |
| Localization       | i18next (English / Arabic)     |

---

## 📁 Project Structure

```text
CustomerSupportCRM/
├── src/
│   ├── BackEnd/
│   │   ├── CustomerSupportCRM.sln
│   │   ├── docs/
│   │   │   └── architecture.md
│   │   └── src/
│   │       ├── CustomerSupportCRM.Domain
│   │       ├── CustomerSupportCRM.Application
│   │       ├── CustomerSupportCRM.Infrastructure
│   │       └── CustomerSupportCRM.API
│   │
│   └── FrontEnd/
│       ├── docs/
│       │   └── frontend-architecture.md
│       └── src/
│           ├── features/        # one folder per feature slice
│           ├── pages/
│           ├── routes/
│           ├── layouts/
│           ├── lib/              # API client, auth, i18n, query setup
│           └── components/ui     # shared presentational components
│
├── .squad/                       # planning artifacts (stories/plans) — not application code
├── .gitignore
└── README.md
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET SDK 10.0](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) 20+ and npm
- SQL Server (LocalDB, Developer, or Express edition)

### 1. Clone the Repository

```bash
git clone <repository-url>
cd CustomerSupportCRM
```

### 2. Backend Configuration

Before running the API, configure the following in
`src/BackEnd/src/CustomerSupportCRM.API/appsettings.Development.json`
(or via environment variables):

- `ConnectionStrings:DefaultConnection` — your SQL Server connection string
- `Jwt:SecretKey` — a strong signing key for local development
- `AdminSeed:Email` / `AdminSeed:Password` — optional, seeds an initial
  Admin account on first run

> These values ship as either empty or clearly-labeled development
> placeholders. Replace them with your own — never commit real credentials.

### 3. Database

Apply the EF Core migrations from the API project directory:

```bash
cd src/BackEnd/src/CustomerSupportCRM.API
dotnet ef database update --project ../CustomerSupportCRM.Infrastructure --startup-project .
```

Then build and run the backend:

```bash
cd src/BackEnd
dotnet restore
dotnet build CustomerSupportCRM.sln

cd src/CustomerSupportCRM.API
dotnet run
```

The API listens on `http://localhost:5025` and `https://localhost:7125`
by default (see `Properties/launchSettings.json`).

### 4. Frontend

```bash
cd src/FrontEnd
npm install
cp .env.example .env.local   # adjust VITE_API_BASE_URL if needed
npm run dev
```

| Command           | Purpose                                     |
| ------------------ | -------------------------------------------- |
| `npm run dev`       | Start the Vite dev server                    |
| `npm run build`     | Type-check and produce a production build    |
| `npm run preview`   | Serve the production build locally           |
| `npm run lint`      | Run ESLint                                   |
| `npm run format`    | Run Prettier                                 |

---

## 🔐 Authentication & Authorization

- **Register** — self-service sign-up creates an account in the `Customer`
  role, linked to a Customer CRM record.
- **Login** — returns a short-lived JWT access token; the refresh token is
  set as an HttpOnly cookie.
- **Access token** — sent as `Authorization: Bearer <token>`.
- **Refresh token** — long-lived, single-use, rotated on every refresh,
  transported only via an HttpOnly/SameSite cookie.
- **Logout** — revokes the refresh token server-side and clears the cookie.

Roles:

- Admin
- Supervisor
- Manager
- Agent
- Customer

Roles are enforced on every protected endpoint. Staff accounts are created
by an Administrator through user management, not self-registration.

---

## 🌍 Localization

- Full **English** and **Arabic** support.
- Automatic **RTL / LTR** layout switching on the frontend.
- Backend validation and error messages localize via `Accept-Language`.
- Frontend UI copy localizes via i18next.

---

## 📚 API Documentation

With the backend running in the `Development` environment, Swagger UI is
available at:

```
https://localhost:7125/swagger
```

(the exact port depends on your local `launchSettings.json`.) Use the
**Authorize** button with a token obtained from `/api/v1/auth/login`.

---

## 🗄️ Database

- **SQL Server**, accessed through **Entity Framework Core**.
- Schema managed entirely through EF Core migrations — see
  `src/BackEnd/src/CustomerSupportCRM.Infrastructure/Persistence/Migrations`.
- Includes ASP.NET Core Identity tables alongside the CRM domain schema
  (customers, tickets, communication channels, knowledge base, reports).
- A Customer CRM record can be linked to its portal `ApplicationUser`
  (one Customer per user, optional).

No database credentials are stored in the repository.

---

## 🔒 Security

- JWT-based authentication with short-lived access tokens.
- Refresh tokens transported only via an **HttpOnly, SameSite** cookie —
  never exposed to frontend JavaScript.
- Single-use refresh-token rotation with **replay detection**.
- Role-based authorization on every protected endpoint.
- Password hashing via **ASP.NET Core Identity**.
- All secrets are supplied through configuration/environment variables —
  no secrets are committed to source control.

---

## 📌 Project Status

```text
Status:        Completed
Features:      Implemented
Backend:       Build passing
Frontend:      Build passing
Database:      Migrations applied
Localization:  Arabic / English
```

---

## 📝 Notes

- Some configuration values (JWT signing key, Admin seed credentials)
  ship as empty or clearly-labeled development placeholders. Set your own
  values before running outside your local machine.
- `.squad/` contains planning artifacts used during development and is not
  part of the running application.

---

## 📄 Documentation

- Backend architecture reference: [`src/BackEnd/docs/architecture.md`](src/BackEnd/docs/architecture.md)
- Frontend architecture reference: [`src/FrontEnd/docs/frontend-architecture.md`](src/FrontEnd/docs/frontend-architecture.md)
