# reports-management — plan overview

Entry point for the **reports-management** feature. Stories execute in order by their `NN` prefix.

## Stories

| NN | File | Title | Tracker id | Depends on |
|----|------|-------|------------|------------|
| _add rows as stories are planned_ |
| 47 | `47-story-ticket-reports.md` | Ticket Reports | ticket-reports | — |
| 48 | `48-story-agent-performance-reports.md` | Agent Performance | agent-performance-reports | — |
| 50 | `50-story-customer-satisfaction-reports.md` | Customer Satisfaction | customer-satisfaction-reports | — |
| 51 | `51-story-management-dashboards.md` | Management Dashboards | management-dashboards | — |

## Dependency notes

### ReportsController Consolidation

All three report stories (47, 48, 50) contribute actions to a **single `ReportsController`** at `api/reports`:
- **Plan 47** creates `ReportsController` with `GET /api/reports/tickets` (TicketReportResponse)
- **Plan 48** augments with `GET /api/reports/agent-performance` (AgentPerformanceReportDto)
- **Plan 50** augments with `GET /api/reports/customer-satisfaction` (CustomerSatisfactionReportDto)

Each story adds its respective service to the controller's constructor. The routes are:
- `GET /api/reports/tickets?fromDate=…&toDate=…`
- `GET /api/reports/agent-performance?from=…&to=…&agentId=…`
- `GET /api/reports/customer-satisfaction?from=…&to=…`

### SLA Performance (FR-047) — Removed

FR-047 — SLA Performance and its story (`sla-performance-reports`, formerly Plan 49) have been removed from scope. Plan 51 (`management-dashboards`) is gated on Plans 47, 48, and 50 only, and ships without an SLA widget.
