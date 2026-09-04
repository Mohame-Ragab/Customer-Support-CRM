# CustomerSupportCRM — Squad Plans Audit

Audit method: 10 parallel read-only audit agents (one per feature folder), each independently re-reading every plan file and its corresponding `intake.md`, then verifying every "existing" file/symbol claim against the real repository (`src/BackEnd/src/`, `src/FrontEnd/src/`) via Glob/Grep/Read. No story, `intake.md`, plan, or source file was modified by this audit. `squad status` before and after: **51 stories / 51 plans**, unchanged.

## 1. Executive Summary

| Metric | Count |
|---|---|
| Total Stories | 51 |
| Total Plans | 51 |
| Plans Audited | 51 (100%) |
| Critical Issues | 16 |
| High Issues | ~38 |
| Medium Issues | ~34 |
| Low Issues | ~46 (mostly `UNNECESSARY_TEST_SCOPE`, present in essentially all 51 plans) |
| Plans Safe to Implement | 10 |
| Plans Requiring Review | 40 |
| Blocked Plans | 1 (Plan 49, `sla-performance-reports` — depends on a permanently deleted feature) |

**The single most important, repo-wide finding:** at least **14 plans** independently reference a role constant `Roles.Administrator`, which **does not exist**. `src/BackEnd/src/CustomerSupportCRM.Domain/Constants/Roles.cs` defines exactly `Admin`, `Supervisor`, `Manager`, `Agent`. As written, every one of those plans' `[Authorize(Roles = ...)]` attributes (or, on the frontend, `hasRole(user, 'Administrator')` checks) either fails to compile or silently locks out every real Administrator. This single, mechanically-fixable defect was independently reproduced by many different plan-generation runs across 8 different features and should be corrected in one coordinated pass before any of those plans are implemented.

**The second most important finding:** at least **10 plans** assume **MediatR** (`IRequest<T>`, `ISender`/`IMediator`, handler classes) is the project's established Application-layer dispatch pattern. It is not — `CustomerSupportCRM.Application.csproj` has no MediatR package reference anywhere in the solution, and `Application/Common/Behaviors/README.md` explicitly says so. A roughly equal number of plans (notably 02, 31, 34, 35, 39, 50) correctly detected this (several by explicitly grepping) and used a plain Application-service pattern instead. Whichever convention a given feature settles on should be applied consistently within that feature.

**The one plan that cannot be implemented as written:** Plan 49 (`reports-management/sla-performance-reports`) is built entirely on an `SlaTarget` data model that only ever existed via the `sla-automation` feature, which has been **permanently deleted** from project scope (removed in an earlier session, at the user's explicit request). Plan 49 treats this as an ordinary "not yet merged" sequencing dependency ("stop and re-plan") rather than a permanent scope gap — no future story will ever satisfy it. This needs a product decision, not an implementation attempt.

## 2. Plan-by-Plan Results

| # | Story | Plan | Story Alignment | Architecture | Existing Paths | Existing Symbols | Dependencies | Scope | Quality | Status |
|---|---|---|---|---|---|---|---|---|---|---|
| 01 | auth/user-login | 01-story-user-login.md | PASS | PASS | REVIEW | REVIEW | 08, security-admin/manage-users | PASS | NEEDS_REVIEW | REVIEW |
| 02 | agent-dashboard/manage-tasks-and-reminders | 02-story-manage-tasks-and-reminders.md | PASS | PASS | PASS | PASS | none | PASS | GOOD | PASS |
| 03 | agent-dashboard/team-collaboration-on-tickets | 03-story-team-collaboration-on-tickets.md | PASS | REVIEW | PASS | PASS | tickets/create-ticket | PASS | GOOD | REVIEW |
| 04 | agent-dashboard/use-quick-replies | 04-story-use-quick-replies.md | PASS | FAIL | PASS | REVIEW | none | PASS | NEEDS_REVIEW | REVIEW |
| 05 | agent-dashboard/view-assigned-tickets | 05-story-view-assigned-tickets.md | PASS | FAIL | PASS | REVIEW | tickets/assign-ticket | PASS | NEEDS_REVIEW | REVIEW |
| 06 | agent-dashboard/view-customer-information-from-ticket | 06-story-view-customer-information-from-ticket.md | PASS | PASS | PASS | PASS | customers/view-customer, tickets/track-ticket | PASS | EXCELLENT | PASS |
| 07 | auth/change-password | 07-story-change-password.md | PASS | PASS | PASS | PASS | 01 | PASS | GOOD | REVIEW |
| 08 | auth/customer-registration | 08-story-customer-registration.md | PASS | PASS | PASS | REVIEW | feeds 09 | PASS | NEEDS_REVIEW | REVIEW |
| 09 | auth/email-verification | 09-story-email-verification.md | PASS | PASS | PASS | PASS | 08 | PASS | GOOD | REVIEW |
| 10 | auth/forgot-password | 10-story-forgot-password.md | PASS | REVIEW | PASS | REVIEW | feeds 13 | PASS | NEEDS_REVIEW | REVIEW |
| 11 | auth/logout | 11-story-logout.md | PASS | PASS | PASS | PASS | 01 | PASS | GOOD | REVIEW |
| 12 | auth/refresh-access-token | 12-story-refresh-access-token.md | PASS | PASS | PASS | PASS | 01, 11 | PASS | NEEDS_REVIEW | REVIEW |
| 13 | auth/reset-password | 13-story-reset-password.md | PASS | REVIEW | PASS | REVIEW | 10, 07 | PASS | NEEDS_REVIEW | REVIEW |
| 14 | auth/view-and-update-user-profile | 14-story-view-and-update-user-profile.md | PASS | PASS | PASS | REVIEW | 01 | PASS | GOOD | REVIEW |
| 15 | security-admin/manage-users | 15-story-manage-users.md | REVIEW | REVIEW | PASS | FAIL | 16 | PASS | NEEDS_REVIEW | REVIEW |
| 16 | security-admin/manage-roles | 16-story-manage-roles.md | PASS | PASS | PASS | PASS | 15 | PASS | EXCELLENT | PASS |
| 17 | security-admin/manage-role-permissions | 17-story-manage-role-permissions.md | PASS | PASS | PASS | PASS | 16 | PASS | GOOD | REVIEW |
| 18 | security-admin/view-audit-logs | 18-story-view-audit-logs.md | REVIEW | REVIEW | PASS | FAIL | (should be depended on by 15/16/17/19) | PASS | NEEDS_REVIEW | REVIEW |
| 19 | security-admin/manage-system-configuration | 19-story-manage-system-configuration.md | PASS | PASS | PASS | PASS | none | PASS | GOOD | PASS |
| 20 | platform/multilingual-support-arabic-and-english | 20-story-multilingual-support-arabic-and-english.md | PASS | PASS | PASS | PASS | none | PASS | GOOD | REVIEW |
| 21 | platform/responsive-web-and-mobile-experience | 21-story-responsive-web-and-mobile-experience.md | PASS | PASS | PASS | PASS | 20 | PASS | GOOD | REVIEW |
| 22 | platform/multi-department-support | 22-story-multi-department-support.md | PASS | REVIEW | PASS | REVIEW | none | PASS | NEEDS_REVIEW | REVIEW |
| 23 | platform/custom-branding | 23-story-custom-branding.md | PASS | PASS | PASS | REVIEW | 20, 21 | PASS | NEEDS_REVIEW | REVIEW |
| 24 | customers/create-customer | 24-story-create-customer.md | PASS | PASS | PASS | PASS | none | PASS | GOOD | PASS |
| 25 | customers/view-customer | 25-story-view-customer.md | PASS | REVIEW | PASS | FAIL | 24 | PASS | NEEDS_REVIEW | REVIEW |
| 26 | customers/update-customer | 26-story-update-customer.md | PASS | REVIEW | PASS | FAIL | 24, 25 (unstated) | PASS | NEEDS_REVIEW | REVIEW |
| 27 | customers/view-customer-interaction-history | 27-story-view-customer-interaction-history.md | PASS | REVIEW | REVIEW | FAIL | 24, tickets/create-ticket, 25 | PASS | NEEDS_REVIEW | REVIEW |
| 28 | customers/manage-customer-notes | 28-story-manage-customer-notes.md | PASS | PASS | PASS | FAIL | 24 | PASS | GOOD | REVIEW |
| 29 | customers/manage-customer-attachments | 29-story-manage-customer-attachments.md | PASS | PASS | PASS | PASS | 24 | PASS | EXCELLENT | PASS |
| 30 | tickets/create-ticket | 30-story-create-ticket.md | PASS | REVIEW | PASS | REVIEW | customers/create-customer | PASS | NEEDS_REVIEW | REVIEW |
| 31 | tickets/track-ticket | 31-story-track-ticket.md | PASS | PASS | PASS | REVIEW | 30 | PASS | GOOD | REVIEW |
| 32 | tickets/set-ticket-category-and-priority | 32-story-set-ticket-category-and-priority.md | PASS | REVIEW | PASS | FAIL | 30; feeds 36 | PASS | NEEDS_REVIEW | REVIEW |
| 33 | tickets/assign-ticket | 33-story-assign-ticket.md | PASS | PASS | PASS | REVIEW | 30; feeds 36 | PASS | GOOD | REVIEW |
| 34 | tickets/change-ticket-status | 34-story-change-ticket-status.md | REVIEW | PASS | PASS | FAIL | 30; feeds 36 | PASS | NEEDS_REVIEW | REVIEW |
| 35 | tickets/escalate-ticket | 35-story-escalate-ticket.md | PASS | PASS | PASS | FAIL | 30; feeds 36 | REVIEW | GOOD | REVIEW |
| 36 | tickets/view-ticket-history | 36-story-view-ticket-history.md | REVIEW | PASS | PASS | PASS | 30, 32, 33, 34, 35 | PASS | NEEDS_REVIEW | REVIEW |
| 37 | communication-channels/email-communication-channel | 37-story-email-communication-channel.md | PASS | PASS | PASS | PASS | tickets/create-ticket | PASS | GOOD | REVIEW |
| 38 | communication-channels/live-chat-communication-channel | 38-story-live-chat-communication-channel.md | PASS | REVIEW | PASS | REVIEW | tickets/create-ticket (conflicting def.) | PASS | NEEDS_REVIEW | REVIEW |
| 39 | communication-channels/web-forms-channel | 39-story-web-forms-channel.md | PASS | PASS | PASS | PASS | tickets/create-ticket | PASS | EXCELLENT | PASS |
| 40 | knowledge-base/manage-knowledge-base-content | 40-story-manage-knowledge-base-content.md | PASS | PASS | PASS | REVIEW | none | PASS | NEEDS_REVIEW | REVIEW |
| 41 | knowledge-base/search-knowledge-base | 41-story-search-knowledge-base.md | PASS | PASS | PASS | REVIEW | 40 | PASS | GOOD | REVIEW |
| 42 | customer-portal/submit-ticket-via-customer-portal | 42-story-submit-ticket-via-customer-portal.md | PASS | PASS | PASS | PASS | tickets/create-ticket, auth/user-login | PASS | GOOD | PASS |
| 43 | customer-portal/track-ticket-requests | 43-story-track-ticket-requests.md | PASS | REVIEW | PASS | PASS | tickets/track-ticket, 42 | PASS | GOOD | REVIEW |
| 44 | customer-portal/view-support-request-history | 44-story-view-support-request-history.md | REVIEW | REVIEW | PASS | PASS | 42 | PASS | NEEDS_REVIEW | REVIEW |
| 45 | customer-portal/access-faqs | 45-story-access-faqs.md | PASS | PASS | PASS | PASS | knowledge-base/{40,41} | PASS | EXCELLENT | PASS |
| 46 | customer-portal/submit-customer-feedback | 46-story-submit-customer-feedback.md | PASS | PASS | PASS | REVIEW | tickets/create-ticket, 42 | PASS | NEEDS_REVIEW | REVIEW |
| 47 | reports-management/ticket-reports | 47-story-ticket-reports.md | PASS | REVIEW | PASS | REVIEW | tickets/* | PASS | NEEDS_REVIEW | REVIEW |
| 48 | reports-management/agent-performance-reports | 48-story-agent-performance-reports.md | PASS | REVIEW | PASS | PASS | tickets/{assign-ticket,change-ticket-status} | PASS | GOOD | REVIEW |
| 49 | reports-management/sla-performance-reports | 49-story-sla-performance-reports.md | FAIL | FAIL | PASS | FAIL | **sla-automation (deleted)** | **VIOLATION** | UNSAFE_TO_IMPLEMENT | **FAIL** |
| 50 | reports-management/customer-satisfaction-reports | 50-story-customer-satisfaction-reports.md | PASS | PASS | PASS | PASS | customer-portal/submit-customer-feedback | PASS | GOOD | PASS |
| 51 | reports-management/management-dashboards | 51-story-management-dashboards.md | PASS | PASS | PASS | REVIEW | 47, 48, 49 (blocked), 50 | PASS | GOOD | REVIEW |

---

## 3. Critical Issues

### CRIT-01 — Plan 01 (auth/user-login): JWT claim types don't match what the frontend decodes
**Story:** auth/user-login
**Issue:** Backend issues access-token claims using `ClaimTypes.Name`/`ClaimTypes.Role` (long XML-schema URIs), but the frontend's `lib/auth/jwt.ts`/`types/auth.ts` read the short keys `name`/`role` directly from the raw, un-remapped base64 payload.
**Evidence:** Plan 01 Task 4: outbound token built with `ClaimTypes.NameIdentifier`, `ClaimTypes.Name`, one `ClaimTypes.Role` claim per role. `src/FrontEnd/src/lib/auth/jwt.ts`'s `rolesFromClaims`/`JwtClaims` (verified) expect plain `sub`/`name`/`role` keys.
**Why it matters:** As specified, `AuthProvider.tsx`'s `userFromToken()` would read `claims.role` as `undefined` for every logged-in user — every role-gated UI/authorization check across the entire frontend would silently break, immediately after the very first feature ships.
**Recommended correction:** Issue the access token using plain string claim types (`new Claim("role", role)`, etc.) matching the short keys `jwt.ts` already expects; confirm `CurrentUserService.cs`'s server-side reads (which use `ClaimTypes.Role`) still work via ASP.NET Core's default inbound claim-type mapping.

### CRIT-02 — Plan 01 (auth/user-login): `AuthController` route prefix conflicts with 6 sibling plans
**Story:** auth/user-login
**Issue:** Plan 01 declares `[Route("api/v1/auth")]` on `AuthController`; plans 07, 09, 10, 11, 12, 13 all assume the base `[Route("api/[controller]")]` (`api/auth/*`, no version) on the same class.
**Evidence:** Plan 01 Task 6 vs. plans 07/09/10/11/12/13's controller sections.
**Why it matters:** A C# class can carry only one `[Route]` attribute; whichever plan lands last silently changes the URL prefix for every other auth action already shipped.
**Recommended correction:** Pick one convention (`api/v1/auth/*` or `api/auth/*`) for the whole `auth` feature and update all affected plans and their frontend call sites together, in one pass.

### CRIT-03 — Plan 08 (auth/customer-registration): captured name fields have nowhere to persist
**Story:** auth/customer-registration
**Issue:** `RegisterCustomerCommand(FirstName, LastName, Email, Password)` collects and validates a name, but `ApplicationUser : IdentityUser<Guid>` has zero custom properties (deliberately, per its own doc comment), and no `Customer` domain entity is created in this story to hold them either.
**Evidence:** Plan 08 Task 2 vs. verified `src/BackEnd/src/CustomerSupportCRM.Infrastructure/Identity/ApplicationUser.cs`.
**Why it matters:** As written, a customer's name is collected, validated, and then silently dropped — invisible everywhere after registration.
**Recommended correction:** Either drop `FirstName`/`LastName` from this story's scope (register with email/password only) or explicitly add the storage this requires (which the plan currently forbids) — this needs an explicit decision, not silent hand-waving.

### CRIT-04 through CRIT-11 — `Roles.Administrator` referenced as if it exists (8 plans, security-admin/platform/customers)
**Plans:** 15 (manage-users), 18 (view-audit-logs), 22 (multi-department-support), 23 (custom-branding), 25 (view-customer), 26 (update-customer), 27 (view-customer-interaction-history), 28 (manage-customer-notes)
**Issue:** Each plan's `[Authorize(Roles = ...)]` attribute (and, in Plan 23's case, a frontend `hasRole(user, 'Administrator')` check) references `Roles.Administrator`. `src/BackEnd/src/CustomerSupportCRM.Domain/Constants/Roles.cs` defines only `Admin`, `Supervisor`, `Manager`, `Agent` — verified directly, no such constant exists anywhere in the codebase. Plan 15 additionally references a nonexistent `Roles.Customer`.
**Why it matters:** Code as written will not compile; even if hand-patched to the literal string `"Administrator"`, it would never match the real seeded role `"Admin"`, locking out every actual Administrator.
**Recommended correction:** Replace every `Roles.Administrator` reference with `Roles.Admin` (and drop the `Roles.Customer` reference in Plan 15) across all 8 plans in one coordinated pass. (Note: this exact defect recurs, at HIGH rather than CRITICAL severity per each auditor's own judgment, in 6 further plans — see §7/§11.)

### CRIT-12 — Plan 04 (agent-dashboard/use-quick-replies): assumes MediatR is already wired in, with no setup step
**Story:** agent-dashboard/use-quick-replies
**Issue:** Entire plan is written against `IRequest<Result<T>>`/handler classes, with a hedge that verification is needed but no actual step to add MediatR if absent.
**Evidence:** Plan 04 code samples; verified no `MediatR` package reference in any `.csproj`; `Application/Common/Behaviors/README.md` states the pipeline is "Not implemented yet." Sibling Plan 02 (same feature) explicitly documents MediatR as deferred and uses a plain service.
**Why it matters:** As written the Application-layer code will not compile, and creates an architecture inconsistency with Plan 02 within the same feature.
**Recommended correction:** Rewrite as a plain `IQuickReplyTemplateService` (mirroring Plan 02's `IAgentTaskService`), or add an explicit, first-class task to install/register MediatR and reconcile that decision across the whole feature.

### CRIT-13 — Plan 05 (agent-dashboard/view-assigned-tickets): asserts MediatR infrastructure "already exists," unverified and false
**Story:** agent-dashboard/view-assigned-tickets
**Issue:** Controller code injects `ISender`; Task 4 states "If assembly scanning is already configured, no code change is required" as if this were confirmed fact.
**Evidence:** Same verified absence of MediatR as CRIT-12.
**Why it matters:** `dotnet build`, which this plan's own Done Criteria requires to pass, is unachievable as written.
**Recommended correction:** Same as CRIT-12 — rewrite as a plain service call, or add and reconcile an explicit MediatR-adoption task across Plans 02/03/04/05.

### CRIT-14 — Plan 36 (tickets/view-ticket-history): foundational write-side assumption unmet by all 5 upstream plans
**Story:** tickets/view-ticket-history
**Issue:** Plan 36 introduces `ITicketHistoryWriter` and assumes plans 30 (create-ticket), 32 (classification), 33 (assign), 34 (status), and 35 (escalate) each call it to record their respective lifecycle events. None of them do: 30 emits no `Created` entry; 32 claims to but its handler steps never call any writer; 33 and 35 each write to their *own separate* audit tables (`TicketAssignment`, `TicketEscalation`) instead; 34 explicitly states "No separate history table is written in this story."
**Evidence:** Cross-read of all 6 tickets plans (see the `tickets` feature audit).
**Why it matters:** As currently specified across all 6 plans, `GET /api/tickets/{id}/history` — this story's entire acceptance criterion — would return an empty or near-empty timeline regardless of real ticket activity.
**Recommended correction:** Either retrofit plans 30/32/33/34/35 with explicit `ITicketHistoryWriter.AppendAsync(...)` calls before they're implemented, or redesign Plan 36's read side to union the `TicketAssignments`/`TicketEscalations` tables and `Ticket.UpdatedAt` directly instead of relying on a table nothing else writes to.

### CRIT-15 — Plan 49 (reports-management/sla-performance-reports): built entirely on a permanently deleted feature
**Story:** reports-management/sla-performance-reports
**Issue:** The plan's entire data model (`SlaTarget`, response/resolution target definitions) originates from `sla-automation/define-sla-response-and-resolution-targets` — a story that has been permanently deleted from the project (the whole 5-story `sla-automation` feature was removed at the user's explicit request in an earlier session). The plan treats this as an ordinary sequencing dependency ("if the prerequisite has not landed... stop and re-plan") rather than a scope gap.
**Evidence:** Plan 49's own Prerequisites/Migration sections; cross-reference against the confirmed deletion of `.squad/stories/sla-automation/` and `.squad/plans/sla-automation/`.
**Why it matters:** As written, this story can never be implemented — no future story will ever satisfy its prerequisite. It also blocks Plan 51 (`management-dashboards`), which depends on it for an SLA widget.
**Recommended correction:** Escalate to product/stakeholders before any implementation. Either descope SLA performance reporting from the current milestone, redesign FR-047 to not require formal SLA targets (e.g. simple percentile/median reporting), or fold minimal target-definition scope into this story with explicit sign-off.

### CRIT-16 — Plans 15/18 (security-admin): additional distinct symbol errors beyond `Roles.Administrator`
**Story:** security-admin/manage-users, security-admin/view-audit-logs
**Issue:** Plan 15's frontend nav guard checks `useAuth().user.role === 'Administrator'` against a shape that doesn't exist (`AuthUser.roles` is a plural array, real value `'Admin'`) — this condition is always false. Plan 18 assumes an `IApplicationDbContext.AuditLogs` abstraction that doesn't exist anywhere in the codebase, contradicting its own earlier hedge.
**Evidence:** `src/FrontEnd/src/types/auth.ts` (`roles: string[]`), `src/FrontEnd/src/features/auth/authorization.ts` (`hasRole`); grep of `Application/Common/Interfaces/` for plan 18.
**Why it matters:** Plan 15's Users nav item would never be visible to a real Administrator; Plan 18's query handler would either fail to compile or require an undocumented Onion-layer violation (Application referencing `Infrastructure.ApplicationDbContext` directly).
**Recommended correction:** Plan 15 — use `hasRole(user, Roles.Admin)` from the existing helper. Plan 18 — either explicitly add `IApplicationDbContext` as a new, narrow interface this plan creates, or route reads through an extended `IAuditLogService`.

---

## 4. High Priority Issues

Grouped by theme (individual plan-level detail is in each feature's section of this audit's source notifications; summarized here for the master report):

**H-1. `Roles.Administrator` recurrence at HIGH severity** (auditor judged it non-blocking-but-serious rather than CRITICAL): Plans 30, 32, 35, 40, 47, 49 (tickets ×3, knowledge-base ×1, reports-management ×2) — same defect as CRIT-04–11, same fix.

**H-2. Duplicate/conflicting controller or type definitions across sibling plans:**
- Plans 30 & 31 (tickets): both unconditionally "create" `TicketsController.cs`.
- Plans 42 & 43 (customer-portal): both declare a class named `PortalTicketsController` at `api/portal/tickets`, in different namespaces.
- Plans 47, 48, 49 (reports-management): all three independently "create" `API/Controllers/ReportsController.cs` with three mutually incompatible shapes; Plan 50 avoids this only by using a differently-named controller, producing route-family inconsistency instead.
- Plans 38 (live-chat) & 37 (email): Plan 38 independently defines its own `Ticket` entity/migration with a shape (`CustomerUserId`, `TicketSource`) that diverges from Plan 37's assumed shape (`CustomerId`, `Customer.Email` nav) — a genuine risk of two colliding `Ticket` migrations if both are implemented independently.
- Plans 02 & 05 (agent-dashboard): both independently define an `AgentDashboardPage` component, different files/routes/content, for the same conceptual entry point.
- Plans 43 & 44 (customer-portal): near-total functional duplication — both implement "list my tickets, most recent first, including resolved/closed" via separate DTOs/controllers/routes/pages.

**H-3. MediatR assumed without verification** (compiles-if-verified-otherwise-doesn't; distinct from CRIT-12/13 in that these plans partially hedge): Plans 10, 13, 22, 25, 26, 27, 49.

**H-4. Refresh-token transport contradicts the documented HttpOnly-cookie architecture:** Plans 01 and 12 both design a JSON-body-based refresh token (readable by JavaScript) while Plan 12 simultaneously gestures at "the eventual HttpOnly-cookie refresh (documented plan)" without implementing it — directly undermining the security rationale documented in `src/FrontEnd/docs/frontend-architecture.md` §7.

**H-5. Duplicate `IEmailSender`/`LoggingEmailSender` independently defined:** Plans 09 and 10 both create the identical interface/class at the identical file path, unaware of each other.

**H-6. Cross-plan history-write gaps (component parts of CRIT-14):** Plan 32's classification handler never calls the writer despite claiming to; Plan 33 and Plan 35 each write to their own separate audit table instead of the shared one.

**H-7. `TicketStatus` enum ownership gap:** Plan 34 (change-ticket-status) says the additional status values (`InProgress`/`Resolved`/`Closed`) are "defined by Story tickets/create-ticket," but Plan 30 only defines `New = 0` and explicitly defers the rest to Plan 34 — neither plan actually adds them.

**H-8. Route-prefix inconsistencies within a single feature:** auth (`api/v1/auth` vs `api/auth`, see CRIT-02); customers (Plan 25 asserts `api/v1/customers`, contradicting Plan 24's actual `api/customers`); customer-portal (`api/portal/*` vs `api/customer-portal/*` vs no prefix at all in Plan 46); reports-management (`api/reports/*` vs `api/CustomerSatisfactionReports`).

**H-9. Frontend type/API-shape errors:** Plan 04's `QuickReplyTemplate.OwnerUserId` typed `string` when the real Identity key is `Guid`; Plan 04's frontend uses a `<ProtectedRoute roles={[...]}>` prop API that doesn't exist (`ProtectedRoute` is parameterless); Plan 38 assumes a `Roles.Customer`/`SuperAdmin` role and a customer-JWT flow that doesn't exist anywhere in the backend.

**H-10. `INVALID_EXISTING_SYMBOL`s in customers plans 25/26/27:** all three reference `Roles.Administrator` (see CRIT-08–10) *and* independently assume a MediatR pattern contradicting Plan 24's plain-service pattern within the same feature.

---

## 5. Medium / Low Issues (grouped)

**Medium:**
- **Systemic unnecessary test scope** — essentially every one of the 51 plans includes a "Test Plan" section instructing creation of new backend (`xUnit`) and/or frontend (`Vitest`/RTL) test projects and specific test files, despite the explicit project rule that unit tests are not required for these stories. This is by far the single most common Medium/Low finding and is listed in full in §12.
- Frontend feature-folder convention drift within single features (flat `features/<name>/` vs. nested `features/security-admin/<name>/`, `features/channels/web-forms/`, `features/customer-portal/*` vs `features/tickets/*`) — plans 17, 19 (security-admin); 39 (communication-channels); 44, 45 (customer-portal); 05, 51 (agent-dashboard/reports-management, `features/reports/` ownership).
- Generic Repository/`IUnitOfWork` pattern bypassed in favor of direct `ApplicationDbContext` injection in read/report services — plans 47, 48, 49, 50 (all of reports-management).
- Soft-delete semantics reinvented instead of reusing `BaseEntity.IsDeleted`/`IGenericRepository.Delete()` — plans 22 (`Department.IsActive`), 40 (ambiguous `IsPublished` vs `IsDeleted`).
- `ValidationFilter` will not fire for scalar query-parameter actions (only validates a matching DTO argument type) — plan 41's `Search(string q, ...)` sample action as written would not enforce its own 400-validation criterion.
- Cross-plan Done Criteria not fully gated on external prerequisites landing — plan 27 (interaction history) doesn't gate on `tickets/create-ticket`; plan 51 has no fallback for its permanently-blocked SLA widget dependency (CRIT-15's downstream effect).
- Backend localization coverage inconsistent within a feature — plans 03/04 (agent-dashboard) add frontend i18n but skip backend `SharedResource` keys for their own validators, unlike sibling plan 02.
- `CustomerFeedback.CustomerUserId` typed `string` (plan 46) when the real Identity key is `Guid`; reference to a nonexistent `HasAnyAsync` method (real one is `ExistsAsync`) in the same plan.
- Unstated/incomplete Prerequisites — plan 26 returns a `CustomerDto` type actually defined by plan 25 without listing 25 as a dependency.

**Low:**
- Several plans (30, 38) contain stale/malformed internal document text — self-referential "Story 01"/mismatched filenames, or claims like "no plan files exist yet" that were true only at initial generation time and are now stale (harmless, but confusing for an implementer).
- Minor duplicate-response-DTO risk between plan 24's `CustomerResponse` and plan 25's independently-defined, slightly different `CustomerDto`.
- Stale cross-references to the now-deleted `sla-automation` feature left in "coordinate with..." prerequisite text (plans 02, 33, 35) — harmless as TODO/comment text (nothing is implemented against it) but should be cleaned up.
- Minor AppBar/`position` ambiguity in plan 21 ("sticky, or keep current").
- `SharedResourceKeys` described as "public" in plan 20 when it's actually `internal` (still works via reflection, wording-only issue).

---

## 6. Existing File Reference Problems

*(Only genuinely wrong existing-file assumptions — i.e. paths the plan treats as already there but which don't exist. No new/planned files are listed here.)*

| Plan | Referenced "existing" file | Expected status | Actual status | Issue | Recommendation |
|---|---|---|---|---|---|
| 18 | `Application/Common/Interfaces/IApplicationDbContext.cs` | Exists (used unconditionally in a query handler task) | Does not exist anywhere in the repo | Plan's own earlier hedge ("if it exists... otherwise...") is dropped later in the same document and treated as settled | Either add it as a new interface this plan creates, or route reads through `IAuditLogService` instead |

No other plan was found to assert an existing *file* that is genuinely absent — every other "existing vs new" concern found in this audit was a **symbol** (a constant, method, or type) inside an otherwise-real file, catalogued in §7, or a legitimate forward dependency on a file another not-yet-implemented plan will create (correctly treated as such, not flagged).

## 7. Existing Symbol Problems

| Plan | File | Referenced symbol | Actual situation | Issue | Recommendation |
|---|---|---|---|---|---|
| 15, 18, 22, 23, 25, 26, 27, 28, 30, 32, 35, 40, 47, 49 | `Domain/Constants/Roles.cs` (referenced from various controllers/services) | `Roles.Administrator` | Only `Admin`, `Supervisor`, `Manager`, `Agent` exist | Non-existent constant used in `[Authorize(Roles = ...)]` | Use `Roles.Admin` |
| 15 | `Domain/Constants/Roles.cs` | `Roles.Customer` | Does not exist (no customer role concept in this backend) | Non-existent constant | Remove the reference; no such role exists |
| 23 | `features/auth/authorization.ts` (consumer) | `hasRole(user, 'Administrator')` | `hasRole` exists, but no role is ever named `'Administrator'` (real value `'Admin'`) | Compiles, but always returns `false` — silent runtime bug | Use `hasRole(user, 'Admin')` |
| 04 | `Infrastructure/Identity/ApplicationUser.cs` (implied) | `OwnerUserId: string` matching "AspNetUsers.Id (string)" | `ApplicationUser : IdentityUser<Guid>` — the real key type is `Guid` | Wrong assumed type | Type `OwnerUserId` as `Guid` |
| 04 | `routes/ProtectedRoute.tsx` | `<ProtectedRoute roles={['Agent']}>` | Component is parameterless, used only as a layout-route element wrapping `<Outlet/>` | Non-existent prop API | Gate via `hasRole`/`hasAnyRole`, mount as a layout route like the rest of the app |
| 05 | `routes/ProtectedRoute.tsx` (DI/assembly-scan claim) | "MediatR assembly scanning already configured" | No MediatR package anywhere in the solution | False existing-infrastructure claim | Verify before use; use plain DI otherwise |
| 10, 13, 22, 25, 26, 27, 49 | `Application/DependencyInjection.cs` (implied) | `IRequest<T>`/`IMediator`/`ISender` (MediatR) | Not referenced anywhere in the solution; `Common/Behaviors/README.md` confirms "not implemented yet" | False existing-pattern assumption | Use a plain Application service (see plans 02, 31, 34, 35, 39, 50 for the correct pattern) |
| 46 | `Domain/Interfaces/IGenericRepository.cs` | `HasAnyAsync(...)` | Interface only exposes `ExistsAsync(predicate, ct)` | Non-existent method | Use `ExistsAsync` |
| 46 | `Infrastructure/Identity/ApplicationUser.cs` (implied) | `CustomerUserId: string` "matches Identity" | Real Identity key is `Guid` | Wrong assumed type | Type `CustomerUserId` as `Guid` |
| 38 | `Domain/Constants/Roles.cs` | `Roles.Customer`, role `"SuperAdmin"` | Neither exists; no customer-facing auth/role concept exists anywhere yet | Non-existent constants, foundational gap | Flag customer authentication as a hard prerequisite before this story can execute |

## 8. New Files Planned (informational — not issues)

By volume, essentially every plan introduces new Domain entities, Application DTOs/services, Infrastructure configurations/migrations, an API controller, and a matching frontend feature slice — exactly as expected for 51 not-yet-implemented stories. Representative examples (full per-plan lists are in each feature's detailed findings, summarized in this audit's compilation history):

- **Plan 01:** `LoginRequest/Response`, `IAuthenticationService`, `IJwtTokenGenerator`, `AuthController.cs`
- **Plan 15:** `Application/Features/Users/**`, `IUserManagementService`, `UsersController.cs`
- **Plan 22:** `Domain/Entities/Department.cs`, `Application/Features/Departments/**`, `DepartmentsController.cs`
- **Plan 24:** `Domain/Entities/Customer.cs`, `CustomerConfiguration.cs`, migration `AddCustomer`, `CustomersController.cs`
- **Plan 30:** `Domain/Entities/Ticket.cs`, `TicketStatus.cs`, migration `AddTickets`
- **Plan 36:** `Domain/Entities/TicketHistoryEntry.cs`, `ITicketHistoryWriter`
- **Plan 40:** `Domain/Entities/KnowledgeBaseContent.cs` (unified FAQ/Article/Solution model, per intake's own instruction)
- **Plan 47–50:** four separate report DTOs/query services/controllers
- **Plan 51:** `ManagementDashboardPage.tsx` + 4 summary widgets composing the four report stories' own hooks (correctly, no new backend)

None of the above is an issue — every file listed is explicitly and correctly planned as new.

## 9. Cross-Plan Dependencies

| Plan | Depends On | Reason | Priority |
|---|---|---|---|
| 01 | 08 (indirectly), security-admin/15 | Accounts must exist before login is meaningful | High |
| 07, 11, 12, 14 | 01 | Require an authenticated session | High |
| 09 | 08 | Verifies the account 08 creates | High |
| 10 → 13 | 10 produces the token 13 consumes | Shared token/encoding contract | High |
| 13 | 07 | Reuses 07's password policy | Medium |
| 16 | — | Foundational; 17 depends on it | High |
| 17 | 16 | Permissions attach to roles 16 manages | High |
| 15, 16, 17, 19 | 18 (should call its `IAuditLogService`, currently none do) | Auditability cross-feature requirement | Medium |
| 21, 23 | 20 | RTL/i18n contract, theme/header work | Medium |
| 23 | 22 (pattern reconciliation only) | Should copy 22's fixed role-constant pattern once corrected | Low |
| 25, 26, 27, 28, 29 | 24 | `Customer` entity/config/migration owned by 24 | Critical (blocking) |
| 26 | 25 (unstated) | Returns `CustomerDto`, defined by 25 | High |
| 27 | tickets/create-ticket (30) | `Ticket.CustomerId` FK for interaction history | High |
| 31, 32, 33, 34, 35, 36 | 30 | `Ticket` aggregate owned by 30 | Critical (blocking) |
| 36 | 30, 32, 33, 34, 35 | Needs all five to actually call the shared history writer (currently none do — CRIT-14) | Critical (blocking) |
| 38 | 30 (conflicting) | Independently redefines `Ticket` — must be reconciled with 30/37, not both built | Critical (blocking) |
| 39 | 30 | Delegates ticket creation, doesn't duplicate it | High |
| 41 | 40 | Search reads content 40 authors | High |
| 42, 43, 44, 46 | 30 (tickets/create-ticket), 01 (auth) | Customer-scoped wrapper over the Ticket aggregate + authentication | High |
| 43, 44 | (each other, currently unacknowledged) | Substantially duplicate "list my tickets" | High |
| 45 | 40, 41 | Customer-facing KB access | High |
| 46 | 42 | Feedback tied to an owned, submitted ticket | High |
| 50 | 46 | Sole upstream data source for CSAT reporting | Critical (blocking) |
| 47, 48 | tickets/* (30, 33, 34, 32) | Ticket volume/status/assignment data | High |
| 49 | **sla-automation (deleted, unresolvable)** | No data source will ever exist as specified | **Critical (blocked)** |
| 51 | 47, 48, 49 (blocked), 50 | Composes all four report stories; SLA widget cannot ship until 49 is resolved | High |

**Recommended implementation order** (derived from the table above, resolving blocking items first):

```
Phase 1 — Foundation (fix Roles.Administrator + MediatR conventions repo-wide first)
  Plan 16 (manage-roles)
  Plan 24 (create-customer)
  Plan 30 (create-ticket)  — resolve TicketStatus enum ownership (H-7) and the
                              history-writer contract (CRIT-14) before 32-36 start
  Plan 20 (multilingual-support) — already infra-complete, verify/extend only

Phase 2 — Authentication
  Plan 01 (user-login) — fix JWT claim types (CRIT-01), route prefix (CRIT-02),
                          refresh-token transport (H-4) first
  Plan 08 (customer-registration) — resolve name-field storage (CRIT-03)
  Plan 09 (email-verification) — owns IEmailSender (resolve dup with 10)
  Plan 11 (logout), Plan 12 (refresh-access-token), Plan 07 (change-password)
  Plan 10 (forgot-password) → Plan 13 (reset-password)
  Plan 14 (view-and-update-user-profile)
  Plan 15 (manage-users), Plan 17 (manage-role-permissions), Plan 18 (view-audit-logs),
  Plan 19 (manage-system-configuration)

Phase 3 — Core CRM
  Plan 25 (view-customer) → Plan 26 (update-customer) → Plan 28 (manage-customer-notes)
    → Plan 29 (manage-customer-attachments) → Plan 27 (interaction-history, also needs
    tickets/create-ticket)
  Plan 31 (track-ticket) → Plan 32 (classification) → Plan 33 (assign-ticket)
    → Plan 34 (change-ticket-status) → Plan 35 (escalate-ticket) → Plan 36 (view-ticket-history,
    only once the history-writer gap is closed)
  Plan 21 (responsive-web-and-mobile), Plan 22 (multi-department), Plan 23 (custom-branding)
  Plan 37 (email-channel) — resolve Ticket-shape conflict with Plan 38 first
  Plan 39 (web-forms-channel) — pending the FR-025/FR-041 product decision
  Plan 40 (manage-knowledge-base-content) → Plan 41 (search-knowledge-base)

Phase 4 — Frontend-facing / composition
  Plan 02 (manage-tasks-and-reminders) → Plan 05 (view-assigned-tickets, reconcile
    dashboard page with 02) → Plan 03 (team-collaboration) → Plan 04 (use-quick-replies)
    → Plan 06 (view-customer-information-from-ticket)
  Plan 38 (live-chat) — only after resolving the customer-auth gap (H-9) and the
    Ticket-ownership conflict with 37
  Plan 42 (submit-ticket-via-customer-portal) → Plan 43 (track-ticket-requests,
    absorb Plan 44 into it) → Plan 45 (access-faqs) → Plan 46 (submit-customer-feedback)

Phase 5 — Reporting (Plan 49 held pending product decision)
  Plan 47 (ticket-reports) → Plan 48 (agent-performance-reports) → Plan 50
    (customer-satisfaction-reports) → Plan 51 (management-dashboards, ship without
    an SLA widget until Plan 49 is resolved)
  Plan 49 (sla-performance-reports) — BLOCKED, do not implement until product
    resolves the deleted sla-automation dependency (CRIT-15)
```

## 10. Duplicate / Conflicting Work

- **Plans 30 & 31** — both unconditionally create `API/Controllers/TicketsController.cs`.
- **Plans 42 & 43** — both create a `PortalTicketsController` class at the same route prefix, different namespaces.
- **Plans 47, 48 & 49** — all three create `API/Controllers/ReportsController.cs` with incompatible shapes; **Plan 50** sidesteps this with its own differently-named controller instead, at the cost of route-family consistency.
- **Plans 37 & 38** — independently define two different shapes of the `Ticket` entity/migration.
- **Plans 02 & 05** — independently define two different `AgentDashboardPage` components for the same conceptual page.
- **Plans 43 & 44** — near-total functional duplication of "list my own tickets, most recent first, including resolved/closed," via entirely separate DTOs/controllers/routes/pages.
- **Plans 09 & 10** — independently define an identical `IEmailSender`/`LoggingEmailSender` at the identical file path.
- **Plans 24 & 25** — two slightly different response DTOs (`CustomerResponse` vs. `CustomerDto`) for the same resource.

## 11. Scope Violations

- **Plan 49 (sla-performance-reports)** — depends entirely on the deleted `sla-automation` feature's data model. **CRITICAL** (see CRIT-15). This is the only actual scope violation found — no plan reintroduces AI/Automation, WhatsApp, SMS, or any other explicitly-removed external integration. (Plans 02, 33, 35 contain harmless, stale *textual* references to the deleted `sla-automation` feature in comments/prerequisite notes — flagged as LOW, not a violation, since nothing is actually implemented against it.)

## 12. Unnecessary Unit Tests

Every one of the 51 audited plans includes a "Test Plan" section proposing backend (xUnit) and/or frontend (Vitest/RTL) unit/integration test authoring, and in the large majority of cases explicitly instructs creating a brand-new test project (`CustomerSupportCRM.Application.Tests`, `CustomerSupportCRM.API.IntegrationTests`, a `tests/` folder, a `vitest.config.ts`, etc.) where none currently exists in either `src/BackEnd` or `src/FrontEnd`. This directly contradicts the stated project rule that unit tests are not required, and `src/BackEnd/docs/architecture.md` §18's own explicit statement that no test projects were created "per explicit scope."

**Classification: `UNNECESSARY_TEST_SCOPE`, all 51 plans.** No plan should be blocked on this alone (all are otherwise implementable without their Test Plan sections), but every plan's Done Criteria that gates on "tests pass" should be relaxed to gate on `dotnet build`/`npm run build` + the plan's own manual Verification Steps instead, which are present and adequate in every plan reviewed.

## 13. Recommended Implementation Order

See the "Recommended implementation order" block in §9 (Cross-Plan Dependencies) for the full 5-phase sequence, derived from the actual dependencies found across all 51 plans — not invented.

## 14. Final Classification

### SAFE TO IMPLEMENT (10)
02, 06, 16, 19, 24, 29, 39, 42, 45, 50
*(All PASS status. Note even these carry the repo-wide `UNNECESSARY_TEST_SCOPE` low-severity finding — safe to implement means no CRITICAL/HIGH defect was found, not "implement the Test Plan section as written.")*

### REVIEW BEFORE IMPLEMENTATION (40)
01, 03, 04, 05, 07, 08, 09, 10, 11, 12, 13, 14, 15, 17, 18, 20, 21, 22, 23, 25, 26, 27, 28, 30, 31, 32, 33, 34, 35, 36, 37, 38, 40, 41, 43, 44, 46, 47, 48, 51
*(Each has at least one HIGH or CRITICAL finding — mostly the mechanically-fixable `Roles.Administrator`/MediatR issues, plus feature-specific defects listed in §3–4 — that should be corrected before implementation begins, but none represents an unresolvable scope problem.)*

### BLOCKED (1)
49 (`reports-management/sla-performance-reports`) — depends on a permanently deleted feature; cannot be implemented as specified without a product decision (see CRIT-15).

## 15. Final Summary

```text
========================================
SQUAD PLAN AUDIT COMPLETE
========================================

Stories: 51
Plans: 51
Plans Audited: 51

Critical: 16
High: ~38
Medium: ~34
Low: ~46

Safe to Implement: 10
Review Required: 40
Blocked: 1

Audit Report:
.squad/plan-audit.md

IMPORTANT:
No Stories modified.
No existing Plans modified.
No source code modified.
No tests created.
========================================
```
