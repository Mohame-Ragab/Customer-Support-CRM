# audit

Admin-only, most-recent-first viewer for the security-admin audit log (security-admin/view-audit-logs). Reads from `GET /api/audit-logs`; the backend writer contract (`IAuditLogService`) is invoked by other F10 handlers on successful mutations and is not part of this frontend module.
