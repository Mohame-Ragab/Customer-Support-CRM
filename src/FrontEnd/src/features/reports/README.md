# reports (F09 — Reports & Management)

Read-only management reporting: ticket volume/status/category/priority, agent
performance, and customer satisfaction. Admin/Supervisor only.

- `api/reportsApi.ts` — thin wrappers over `apiClient` for the three
  `GET /api/reports/*` endpoints.
- `hooks/` — one `useQuery` hook per report.
- `components/` — dashboard summary widgets (`TicketSummaryWidget`,
  `AgentPerformanceSummaryWidget`, `CsatSummaryWidget`), each an
  independent presentational + own-query unit.
- `pages/ManagementDashboardPage.tsx` — composes the three widgets at
  `/reports/dashboard`.
- `pages/AgentPerformancePage.tsx` — the full agent-performance report with a
  date-range filter, at `/reports/agent-performance`.

FR-047 (SLA Performance) was removed from scope — there is no SLA widget,
endpoint, or report here, and none should be added.
