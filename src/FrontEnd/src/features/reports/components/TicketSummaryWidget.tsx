import { useTranslation } from 'react-i18next'
import { Card, CardContent, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicketReport } from '../hooks/useTicketReport'
import { defaultDateRange } from '../dateRange'

const { from, to } = defaultDateRange()

/** Presentational + own-query unit: no props from the page, so widgets can be independently rearranged. */
export function TicketSummaryWidget() {
  const { t, i18n } = useTranslation()
  const { data, isPending, isError, error, refetch } = useTicketReport(from, to)

  return (
    <Card>
      <CardContent>
        <Typography variant="subtitle1" gutterBottom>
          {t('pages.dashboard.widgets.tickets.title')}
        </Typography>

        {isPending && <LoadingState />}
        {isError && (
          <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
        )}
        {!isPending && !isError && data && data.totalTickets === 0 && (
          <EmptyState
            title={t('pages.dashboard.widgets.noData', { defaultValue: 'No data for this period' })}
          />
        )}
        {!isPending && !isError && data && data.totalTickets > 0 && (
          <Stack spacing={0.5}>
            <Typography variant="h4">
              {new Intl.NumberFormat(i18n.language).format(data.totalTickets)}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t('reports.tickets.total', { defaultValue: 'Total tickets (last 30 days)' })}
            </Typography>
          </Stack>
        )}
      </CardContent>
    </Card>
  )
}
