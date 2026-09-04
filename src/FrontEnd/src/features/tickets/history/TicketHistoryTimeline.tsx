import { useTranslation } from 'react-i18next'
import { Box, List, ListItem, ListItemText, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicketHistory } from './useTicketHistory'
import { TicketHistoryEventType } from './types'

interface TicketHistoryTimelineProps {
  ticketId: string
}

const EVENT_LABEL_KEYS: Record<number, string> = {
  [TicketHistoryEventType.Created]: 'tickets.history.events.created',
  [TicketHistoryEventType.CategoryChanged]: 'tickets.history.events.categoryChanged',
  [TicketHistoryEventType.PriorityChanged]: 'tickets.history.events.priorityChanged',
  [TicketHistoryEventType.AssignmentChanged]: 'tickets.history.events.assignmentChanged',
  [TicketHistoryEventType.StatusChanged]: 'tickets.history.events.statusChanged',
  [TicketHistoryEventType.Escalated]: 'tickets.history.events.escalated',
}

export function TicketHistoryTimeline({ ticketId }: TicketHistoryTimelineProps) {
  const { t } = useTranslation()
  const { data: entries, isLoading, isError, error, refetch } = useTicketHistory(ticketId)

  if (isLoading) {
    return <LoadingState />
  }

  if (isError) {
    return (
      <ErrorState
        title={t('errors.unexpected')}
        message={normalizeApiError(error).detail}
        onRetry={() => refetch()}
      />
    )
  }

  if (!entries || entries.length === 0) {
    return <EmptyState title={t('tickets.history.empty')} />
  }

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        {t('tickets.history.title')}
      </Typography>
      <List>
        {entries.map((entry) => (
          <ListItem key={entry.id} divider>
            <ListItemText
              primary={t(EVENT_LABEL_KEYS[entry.eventType] ?? 'tickets.history.events.created')}
              secondary={
                <>
                  <Typography
                    component="span"
                    variant="caption"
                    color="text.secondary"
                    sx={{ display: 'block' }}
                  >
                    {new Date(entry.occurredAt).toLocaleString()} —{' '}
                    {entry.actorDisplayName ?? t('tickets.history.system')}
                  </Typography>
                  {(entry.oldValue || entry.newValue) && (
                    <Typography component="span" variant="body2" color="text.secondary">
                      {entry.oldValue ?? '—'} → {entry.newValue ?? '—'}
                    </Typography>
                  )}
                  {entry.note && (
                    <Typography
                      component="span"
                      variant="body2"
                      color="text.secondary"
                      sx={{ display: 'block' }}
                    >
                      {entry.note}
                    </Typography>
                  )}
                </>
              }
            />
          </ListItem>
        ))}
      </List>
    </Box>
  )
}
