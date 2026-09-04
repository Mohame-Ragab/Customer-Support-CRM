import { Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  Box,
  Chip,
  Link,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useMyAssignedTickets } from '../hooks/useMyAssignedTickets'
import { TicketPriority, TicketStatus } from '../types'

const STATUS_LABEL_KEYS: Record<TicketStatus, string> = {
  [TicketStatus.New]: 'tickets.statusLabels.new',
  [TicketStatus.InProgress]: 'tickets.statusLabels.inProgress',
  [TicketStatus.Resolved]: 'tickets.statusLabels.resolved',
  [TicketStatus.Closed]: 'tickets.statusLabels.closed',
}

const PRIORITY_LABEL_KEYS: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'tickets.classification.priorityLevels.low',
  [TicketPriority.Medium]: 'tickets.classification.priorityLevels.medium',
  [TicketPriority.High]: 'tickets.classification.priorityLevels.high',
  [TicketPriority.Urgent]: 'tickets.classification.priorityLevels.urgent',
}

export function AssignedTicketsSection() {
  const { t } = useTranslation()
  const { data, isPending, isError, error, refetch } = useMyAssignedTickets()

  if (isPending) {
    return <LoadingState />
  }

  if (isError) {
    return (
      <ErrorState
        title={t('agentDashboard.tickets.errorTitle', { defaultValue: 'Error' })}
        message={normalizeApiError(error).detail ?? t('agentDashboard.tickets.ticketsError')}
        onRetry={() => refetch()}
      />
    )
  }

  const tickets = data?.items ?? []

  if (tickets.length === 0) {
    return <EmptyState title={t('agentDashboard.tickets.emptyAssignedTickets')} />
  }

  return (
    <TableContainer>
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>{t('agentDashboard.tickets.columns.reference')}</TableCell>
            <TableCell>{t('agentDashboard.tickets.columns.subject')}</TableCell>
            <TableCell>{t('agentDashboard.tickets.columns.status')}</TableCell>
            <TableCell>{t('agentDashboard.tickets.columns.priority')}</TableCell>
            <TableCell>{t('agentDashboard.tickets.columns.created')}</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {tickets.map((ticket) => (
            <TableRow key={ticket.id} hover>
              <TableCell>
                <Link component={RouterLink} to={`/tickets/${ticket.id}`}>
                  {ticket.id.slice(0, 8)}
                </Link>
              </TableCell>
              <TableCell dir="auto">
                <Box sx={{ maxWidth: 320 }}>
                  <Typography variant="body2" noWrap title={ticket.subject}>
                    {ticket.subject}
                  </Typography>
                </Box>
              </TableCell>
              <TableCell>{t(STATUS_LABEL_KEYS[ticket.status])}</TableCell>
              <TableCell>
                {ticket.priority !== null ? (
                  <Chip size="small" label={t(PRIORITY_LABEL_KEYS[ticket.priority])} />
                ) : (
                  '—'
                )}
              </TableCell>
              <TableCell>{new Date(ticket.createdAt).toLocaleString()}</TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  )
}
