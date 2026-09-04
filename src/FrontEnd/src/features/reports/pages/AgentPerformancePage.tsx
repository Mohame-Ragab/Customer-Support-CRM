import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Container,
  Stack,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAgentPerformance } from '../hooks/useAgentPerformance'
import { defaultDateRange } from '../dateRange'

export function AgentPerformancePage() {
  const { t, i18n } = useTranslation()
  const initial = defaultDateRange()
  const [from, setFrom] = useState(initial.from)
  const [to, setTo] = useState(initial.to)

  // TODO(agent-filter): a dedicated "list agents" endpoint doesn't exist yet;
  // the optional per-agent filter from the plan is deferred until one does.
  const { data, isPending, isError, error, refetch } = useAgentPerformance({ from, to })

  const numberFormat = new Intl.NumberFormat(i18n.language)
  const hoursFormat = new Intl.NumberFormat(i18n.language, { maximumFractionDigits: 1 })

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('reports.agentPerformance.title')}
      </Typography>

      <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} sx={{ mb: 3 }}>
        <TextField
          label={t('reports.agentPerformance.from')}
          type="date"
          value={from}
          onChange={(e) => setFrom(e.target.value)}
          slotProps={{ inputLabel: { shrink: true } }}
        />
        <TextField
          label={t('reports.agentPerformance.to')}
          type="date"
          value={to}
          onChange={(e) => setTo(e.target.value)}
          slotProps={{ inputLabel: { shrink: true } }}
        />
      </Stack>

      {isPending && <LoadingState />}
      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}
      {!isPending && !isError && (data?.rows.length ?? 0) === 0 && (
        <EmptyState title={t('reports.agentPerformance.noData')} />
      )}
      {!isPending && !isError && (data?.rows.length ?? 0) > 0 && (
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>{t('reports.agentPerformance.agent')}</TableCell>
              <TableCell align="right">{t('reports.agentPerformance.ticketsAssigned')}</TableCell>
              <TableCell align="right">{t('reports.agentPerformance.ticketsResolved')}</TableCell>
              <TableCell align="right">
                {t('reports.agentPerformance.avgResolutionHours')}
              </TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {data?.rows.map((row) => (
              <TableRow key={row.agentId}>
                <TableCell dir="auto">{row.agentUserName}</TableCell>
                <TableCell align="right">{numberFormat.format(row.ticketsAssigned)}</TableCell>
                <TableCell align="right">{numberFormat.format(row.ticketsResolved)}</TableCell>
                <TableCell align="right">
                  {row.averageResolutionHours === null
                    ? '—'
                    : hoursFormat.format(row.averageResolutionHours)}
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      )}
    </Container>
  )
}
