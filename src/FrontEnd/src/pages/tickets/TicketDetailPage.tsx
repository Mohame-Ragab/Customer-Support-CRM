import { useParams } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Box, Container, Typography, Card, CardContent, Stack, Divider } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicket } from '@/features/tickets/hooks/useTicketQuery'
import { TicketClassificationEditor } from '@/features/tickets/components/TicketClassificationEditor'
import { TicketHistoryTimeline } from '@/features/tickets/history/TicketHistoryTimeline'
import { TicketMessageTimeline } from '@/features/tickets/messages/TicketMessageTimeline'
import { EmailReplyComposer } from '@/features/tickets/messages/EmailReplyComposer'
import { CustomerInformationPanel } from '@/features/tickets/components/CustomerInformationPanel'
import { InternalCommentThread } from '@/features/tickets/internalComments'

/**
 * Minimal ticket-detail page (no F02 ticket-detail page existed yet - this is
 * the first one, added by F03 email-communication-channel since its UI needs
 * somewhere to live). A full agent working surface with inline
 * assign/status/escalate controls is out of scope here - it belongs to the
 * separate agent-dashboard feature (F04).
 */
export function TicketDetailPage() {
  const { t } = useTranslation()
  const { ticketId } = useParams<{ ticketId: string }>()
  const { data: ticket, isLoading, isError, error } = useTicket(ticketId ?? '')

  if (isLoading) {
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <LoadingState />
      </Container>
    )
  }

  if (isError || !ticket) {
    return (
      <Container maxWidth="md" sx={{ py: 4 }}>
        <ErrorState
          title={t('errors.unexpected')}
          message={isError ? normalizeApiError(error).detail : undefined}
        />
      </Container>
    )
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {ticket.subject}
      </Typography>
      {ticket.description && (
        <Typography variant="body1" color="text.secondary" gutterBottom>
          {ticket.description}
        </Typography>
      )}

      <Box
        sx={{
          mt: 2,
          display: 'flex',
          flexDirection: { xs: 'column', md: 'row' },
          gap: 3,
          alignItems: 'flex-start',
        }}
      >
        <Stack spacing={3} sx={{ flex: 2, minWidth: 0, width: '100%' }}>
          <Card>
            <CardContent>
              <TicketClassificationEditor
                ticketId={ticket.id}
                initialCategoryId={ticket.categoryId}
                initialPriority={ticket.priority}
              />
            </CardContent>
          </Card>

          <Card>
            <CardContent>
              <Typography variant="subtitle1" gutterBottom>
                {t('emailChannel.title', { defaultValue: 'Email' })}
              </Typography>
              <TicketMessageTimeline ticketId={ticket.id} />
              <Divider sx={{ my: 2 }} />
              <EmailReplyComposer ticketId={ticket.id} ticketSubject={ticket.subject} />
            </CardContent>
          </Card>

          <Card>
            <CardContent>
              <InternalCommentThread ticketId={ticket.id} />
            </CardContent>
          </Card>

          <Card>
            <CardContent>
              <TicketHistoryTimeline ticketId={ticket.id} />
            </CardContent>
          </Card>
        </Stack>

        <Box sx={{ flex: 1, minWidth: 0, width: '100%' }}>
          <Card>
            <CardContent>
              <CustomerInformationPanel customerId={ticket.customerId} />
            </CardContent>
          </Card>
        </Box>
      </Box>
    </Container>
  )
}
