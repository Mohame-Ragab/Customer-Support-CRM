import { BrowserRouter, Route, Routes } from 'react-router-dom'

import { AppLayout } from '@/layouts/AppLayout'
import { AuthLayout } from '@/layouts/AuthLayout'
import { HomePage } from '@/pages/HomePage'
import { LoginPage } from '@/pages/LoginPage'
import { NotFoundPage } from '@/pages/NotFoundPage'
import { RegisterPage } from '@/pages/RegisterPage'
import { SettingsPage } from '@/pages/SettingsPage'
import { VerifyEmailPage } from '@/pages/VerifyEmailPage'
import { ForgotPasswordPage } from '@/pages/ForgotPasswordPage'
import { ResetPasswordPage } from '@/pages/ResetPasswordPage'
import { UsersListPage } from '@/features/users/pages/UsersListPage'
import { RolePermissionsPage } from '@/pages/admin/RolePermissionsPage'
import { AuditLogsPage } from '@/pages/AuditLogsPage'
import { SystemConfigurationPage } from '@/pages/admin/SystemConfigurationPage'
import { BrandingPage } from '@/pages/admin/BrandingPage'
import { CustomersListPage } from '@/pages/customers/CustomersListPage'
import { CustomerDetailPage } from '@/pages/customers/CustomerDetailPage'
import { TicketDetailPage } from '@/pages/tickets/TicketDetailPage'
import { CustomerChatPage } from '@/pages/CustomerChatPage'
import { AgentChatQueuePage } from '@/pages/AgentChatQueuePage'
import { WebFormPage } from '@/features/channels/web-forms/pages/WebFormPage'
import { AgentDashboardPage } from '@/pages/AgentDashboardPage'
import { QuickRepliesPage } from '@/features/quick-replies/pages/QuickRepliesPage'
import { ContentListPage } from '@/pages/knowledge-base/ContentListPage'
import { ContentDetailPage } from '@/pages/knowledge-base/ContentDetailPage'
import { SubmitTicketPage } from '@/pages/portal/SubmitTicketPage'
import { MyTicketsPage } from '@/pages/portal/MyTicketsPage'
import { MyTicketDetailPage } from '@/pages/portal/MyTicketDetailPage'
import { FaqsPage } from '@/pages/portal/FaqsPage'
import { FaqDetailPage } from '@/pages/portal/FaqDetailPage'
import { ManagementDashboardPage } from '@/features/reports/pages/ManagementDashboardPage'
import { AgentPerformancePage } from '@/features/reports/pages/AgentPerformancePage'
import { Roles } from '@/features/auth/roles'

import { ProtectedRoute } from './ProtectedRoute'
import { PublicRoute } from './PublicRoute'

/**
 * Routing only - no data fetching, no business logic. CRM feature routes are
 * added under the ProtectedRoute branch once those features exist; none
 * exist yet, so "/" is a placeholder page.
 */
export function AppRouter() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<PublicRoute />}>
          <Route element={<AuthLayout />}>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/verify-email" element={<VerifyEmailPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
          </Route>

          {/* F03 web-forms-channel: public, unauthenticated ticket intake - no AuthLayout (no login form here). */}
          <Route path="/support/submit" element={<WebFormPage />} />
        </Route>

        <Route element={<ProtectedRoute />}>
          <Route element={<AppLayout />}>
            <Route path="/" element={<HomePage />} />
            <Route path="/settings" element={<SettingsPage />} />
          </Route>
        </Route>

        <Route element={<ProtectedRoute allowedRoles={[Roles.Admin]} />}>
          <Route element={<AppLayout />}>
            <Route path="/users" element={<UsersListPage />} />
            <Route path="/admin/role-permissions" element={<RolePermissionsPage />} />
            <Route path="/admin/audit-logs" element={<AuditLogsPage />} />
            <Route path="/admin/system-configuration" element={<SystemConfigurationPage />} />
            <Route path="/admin/branding" element={<BrandingPage />} />
          </Route>
        </Route>

        <Route
          element={
            <ProtectedRoute
              allowedRoles={[Roles.Admin, Roles.Supervisor, Roles.Manager, Roles.Agent]}
            />
          }
        >
          <Route element={<AppLayout />}>
            <Route path="/customers" element={<CustomersListPage />} />
            <Route path="/customers/:id" element={<CustomerDetailPage />} />
            <Route path="/tickets/:ticketId" element={<TicketDetailPage />} />
            <Route path="/agent/chats" element={<AgentChatQueuePage />} />
            <Route path="/dashboard" element={<AgentDashboardPage />} />
            <Route path="/quick-replies" element={<QuickRepliesPage />} />
          </Route>
        </Route>

        {/* F06 knowledge-base: matches KnowledgeBaseContentController's [Authorize(Roles = Agent,Supervisor,Admin)] - no Manager. */}
        <Route
          element={<ProtectedRoute allowedRoles={[Roles.Admin, Roles.Supervisor, Roles.Agent]} />}
        >
          <Route element={<AppLayout />}>
            <Route path="/knowledge-base" element={<ContentListPage />} />
            <Route path="/knowledge-base/:id" element={<ContentDetailPage />} />
          </Route>
        </Route>

        {/*
          F09 reports-management: matches ReportsController's
          [Authorize(Roles = Admin,Supervisor)] - no Manager/Agent. Route is
          "/reports/dashboard" (not "/dashboard") because that path is already
          the F04 Agent Dashboard, open to every staff role.
        */}
        <Route element={<ProtectedRoute allowedRoles={[Roles.Admin, Roles.Supervisor]} />}>
          <Route element={<AppLayout />}>
            <Route path="/reports/dashboard" element={<ManagementDashboardPage />} />
            <Route path="/reports/agent-performance" element={<AgentPerformancePage />} />
          </Route>
        </Route>

        {/* F08 customer-portal: self-service ticket submission/tracking, feedback, and FAQ browsing. */}
        <Route element={<ProtectedRoute allowedRoles={[Roles.Customer]} />}>
          <Route element={<AppLayout />}>
            <Route path="/chat" element={<CustomerChatPage />} />
            <Route path="/portal/tickets" element={<MyTicketsPage />} />
            <Route path="/portal/tickets/new" element={<SubmitTicketPage />} />
            <Route path="/portal/tickets/:id" element={<MyTicketDetailPage />} />
            <Route path="/portal/faqs" element={<FaqsPage />} />
            <Route path="/portal/faqs/:id" element={<FaqDetailPage />} />
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  )
}
