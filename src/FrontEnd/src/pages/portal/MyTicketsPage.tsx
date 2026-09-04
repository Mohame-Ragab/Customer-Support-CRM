import { useState } from 'react'
import { useNavigate, Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import {
  Box,
  Button,
  Container,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useMyPortalTickets } from '@/features/customer-portal/tickets/hooks'
import { TicketStatus } from '@/features/tickets/types'

const STATUS_LABEL_KEYS: Record<TicketStatus, string> = {
  [TicketStatus.New]: 'tickets.statusLabels.new',
  [TicketStatus.InProgress]: 'tickets.statusLabels.inProgress',
  [TicketStatus.Resolved]: 'tickets.statusLabels.resolved',
  [TicketStatus.Closed]: 'tickets.statusLabels.closed',
}

export function MyTicketsPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(20)

  const { data, isLoading, isError, error, refetch } = useMyPortalTickets({
    page: page + 1,
    pageSize,
  })

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          {t('portal.myTickets.title')}
        </Typography>
        <Button variant="contained" component={RouterLink} to="/portal/tickets/new">
          {t('portal.submitTicket.title')}
        </Button>
      </Stack>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.items.length === 0 && (
        <EmptyState title={t('portal.myTickets.empty')} />
      )}

      {!isLoading && !isError && data && data.items.length > 0 && (
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('agentDashboard.tickets.columns.subject')}</TableCell>
                <TableCell>{t('agentDashboard.tickets.columns.status')}</TableCell>
                <TableCell>{t('agentDashboard.tickets.columns.created')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.items.map((ticket) => (
                <TableRow
                  key={ticket.id}
                  hover
                  onClick={() => navigate(`/portal/tickets/${ticket.id}`)}
                  sx={{ cursor: 'pointer' }}
                >
                  <TableCell dir="auto">{ticket.subject}</TableCell>
                  <TableCell>{t(STATUS_LABEL_KEYS[ticket.status])}</TableCell>
                  <TableCell>{new Date(ticket.createdAt).toLocaleString()}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={data.totalCount}
            page={page}
            onPageChange={(_, newPage) => setPage(newPage)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => {
              setPageSize(parseInt(e.target.value, 10))
              setPage(0)
            }}
          />
        </Box>
      )}
    </Container>
  )
}
