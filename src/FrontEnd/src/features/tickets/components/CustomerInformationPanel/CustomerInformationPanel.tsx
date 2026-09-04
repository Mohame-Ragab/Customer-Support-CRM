import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Box, Tab, Tabs, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { ErrorBoundary } from '@/components/common/ErrorBoundary'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useCustomer } from '@/features/customers/hooks/useCustomerQuery'
import { CustomerInteractionHistoryPanel } from '@/features/customers/interactionHistory/CustomerInteractionHistoryPanel'
import { CustomerNotesPanel } from '@/features/customers/notes/CustomerNotesPanel'
import { CustomerAttachmentsPanel } from '@/features/customers/attachments/CustomerAttachmentsPanel'

interface CustomerInformationPanelProps {
  customerId: string | null | undefined
}

type TabKey = 'profile' | 'history' | 'notes' | 'attachments'

/**
 * Embeds the customer's context (F01) inside the ticket workspace (F04
 * agent-dashboard/view-customer-information-from-ticket) - it composes
 * existing customer-feature pieces rather than re-implementing them. The
 * ticket workspace wraps this in <ErrorBoundary> itself is unnecessary since
 * this component already wraps its own risky subtree; kept here too for
 * defense in depth per the story's "read-only, isolated failure" requirement.
 */
export function CustomerInformationPanel({ customerId }: CustomerInformationPanelProps) {
  const { t } = useTranslation()
  const [tab, setTab] = useState<TabKey>('profile')

  if (!customerId) {
    return (
      <EmptyState
        title={t('tickets.customerPanel.errors.noCustomer', {
          defaultValue: 'No customer linked to this ticket',
        })}
      />
    )
  }

  return (
    <ErrorBoundary>
      <Box>
        <Typography variant="subtitle1" gutterBottom>
          {t('tickets.customerPanel.title')}
        </Typography>

        <Tabs
          value={tab}
          onChange={(_, value: TabKey) => setTab(value)}
          variant="scrollable"
          scrollButtons="auto"
        >
          <Tab value="profile" label={t('tickets.customerPanel.tabs.profile')} />
          <Tab value="history" label={t('tickets.customerPanel.tabs.history')} />
          <Tab value="notes" label={t('tickets.customerPanel.tabs.notes')} />
          <Tab value="attachments" label={t('tickets.customerPanel.tabs.attachments')} />
        </Tabs>

        <Box sx={{ mt: 2 }}>
          {tab === 'profile' && <ProfileTab customerId={customerId} />}
          {tab === 'history' && <CustomerInteractionHistoryPanel customerId={customerId} />}
          {tab === 'notes' && <CustomerNotesPanel customerId={customerId} />}
          {tab === 'attachments' && <CustomerAttachmentsPanel customerId={customerId} />}
        </Box>
      </Box>
    </ErrorBoundary>
  )
}

function ProfileTab({ customerId }: { customerId: string }) {
  const { t } = useTranslation()
  const { data: customer, isLoading, isError, error, refetch } = useCustomer(customerId)

  if (isLoading) {
    return <LoadingState />
  }

  if (isError) {
    return (
      <ErrorState
        title={t('tickets.customerPanel.errors.loadFailed')}
        message={normalizeApiError(error).detail}
        onRetry={() => refetch()}
      />
    )
  }

  if (!customer) {
    return null
  }

  return (
    <Box dir="auto">
      <Typography variant="h6">
        {customer.firstName} {customer.lastName}
      </Typography>
      {customer.companyName && (
        <Typography variant="body2" color="text.secondary">
          {customer.companyName}
        </Typography>
      )}
      <Box sx={{ mt: 1 }}>
        <Typography variant="body2">{customer.email}</Typography>
        {customer.phoneNumber && <Typography variant="body2">{customer.phoneNumber}</Typography>}
      </Box>
    </Box>
  )
}
