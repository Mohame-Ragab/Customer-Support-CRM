import { useTranslation } from 'react-i18next'
import { Box, Chip, List, ListItem, ListItemText, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicketMessages } from './useTicketMessages'
import { TicketMessageDirection, EmailDeliveryStatus } from './types'

interface TicketMessageTimelineProps {
  ticketId: string
}

const STATUS_COLOR: Record<EmailDeliveryStatus, 'default' | 'success' | 'error' | 'info'> = {
  [EmailDeliveryStatus.Pending]: 'default',
  [EmailDeliveryStatus.Sent]: 'success',
  [EmailDeliveryStatus.Failed]: 'error',
  [EmailDeliveryStatus.Received]: 'info',
}

const STATUS_LABEL_KEYS: Record<EmailDeliveryStatus, string> = {
  [EmailDeliveryStatus.Pending]: 'emailChannel.statusPending',
  [EmailDeliveryStatus.Sent]: 'emailChannel.statusSent',
  [EmailDeliveryStatus.Failed]: 'emailChannel.statusFailed',
  [EmailDeliveryStatus.Received]: 'emailChannel.statusReceived',
}

export function TicketMessageTimeline({ ticketId }: TicketMessageTimelineProps) {
  const { t } = useTranslation()
  const { data: messages, isLoading, isError, error, refetch } = useTicketMessages(ticketId)

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

  if (!messages || messages.length === 0) {
    return <EmptyState title={t('emailChannel.empty', { defaultValue: 'No messages yet' })} />
  }

  return (
    <List>
      {messages.map((message) => (
        <ListItem key={message.id} divider alignItems="flex-start">
          <ListItemText
            primary={
              <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', flexWrap: 'wrap' }}>
                <Chip
                  size="small"
                  label={t(
                    message.direction === TicketMessageDirection.Outbound
                      ? 'emailChannel.outboundBadge'
                      : 'emailChannel.inboundBadge',
                  )}
                />
                <Chip
                  size="small"
                  color={STATUS_COLOR[message.deliveryStatus]}
                  label={t(STATUS_LABEL_KEYS[message.deliveryStatus])}
                />
                <Typography component="span" variant="subtitle2">
                  {message.subject}
                </Typography>
              </Box>
            }
            secondary={
              <>
                <Typography
                  component="span"
                  variant="caption"
                  color="text.secondary"
                  sx={{ display: 'block' }}
                >
                  {message.fromAddress} → {message.toAddress} ·{' '}
                  {new Date(message.createdAt).toLocaleString()}
                </Typography>
                <Typography
                  component="span"
                  variant="body2"
                  sx={{ whiteSpace: 'pre-wrap', display: 'block', mt: 0.5 }}
                >
                  {message.bodyText}
                </Typography>
                {message.failureReason && (
                  <Typography
                    component="span"
                    variant="body2"
                    color="error"
                    sx={{ display: 'block' }}
                  >
                    {message.failureReason}
                  </Typography>
                )}
              </>
            }
          />
        </ListItem>
      ))}
    </List>
  )
}
