import { useParams, Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Button, Card, CardContent, Container, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useMyPortalTicket } from '@/features/customer-portal/tickets/hooks'
import { FeedbackForm } from '@/features/customer-portal/feedback/FeedbackForm'
import { TicketStatus } from '@/features/tickets/types'

const STATUS_LABEL_KEYS: Record<TicketStatus, string> = {
  [TicketStatus.New]: 'tickets.statusLabels.new',
  [TicketStatus.InProgress]: 'tickets.statusLabels.inProgress',
  [TicketStatus.Resolved]: 'tickets.statusLabels.resolved',
  [TicketStatus.Closed]: 'tickets.statusLabels.closed',
}

export function MyTicketDetailPage() {
  const { t } = useTranslation()
  const { id } = useParams<{ id: string }>()
  const { data: ticket, isLoading, isError, error, refetch } = useMyPortalTicket(id ?? '')

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <LoadingState />
      </Container>
    )
  }

  if (isError) {
    const apiError = normalizeApiError(error)
    if (apiError.status === 404) {
      return (
        <Container maxWidth="md" sx={{ py: 4 }}>
          <EmptyState
            title={t('portal.myTickets.notFound', { defaultValue: 'Ticket not found' })}
            action={
              <Button component={RouterLink} to="/portal/tickets">
                {t('customers.detail.backToList', { defaultValue: 'Back to list' })}
              </Button>
            }
          />
        </Container>
      )
    }
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <ErrorState
          title={t('errors.unexpected')}
          message={apiError.detail}
          onRetry={() => refetch()}
        />
      </Container>
    )
  }

  if (!ticket) {
    return null
  }

  const canLeaveFeedback =
    ticket.status === TicketStatus.Resolved || ticket.status === TicketStatus.Closed

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom dir="auto">
        {ticket.subject}
      </Typography>

      <Stack spacing={3}>
        <Card>
          <CardContent>
            <Typography variant="body2" color="text.secondary">
              {t('agentDashboard.tickets.columns.status')}: {t(STATUS_LABEL_KEYS[ticket.status])}
            </Typography>
            {ticket.description && (
              <Typography variant="body1" sx={{ mt: 2, whiteSpace: 'pre-wrap' }} dir="auto">
                {ticket.description}
              </Typography>
            )}
          </CardContent>
        </Card>

        {canLeaveFeedback && (
          <Card>
            <CardContent>
              <FeedbackForm ticketId={ticket.id} />
            </CardContent>
          </Card>
        )}
      </Stack>
    </Container>
  )
}
