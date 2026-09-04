import { useTranslation } from 'react-i18next'
import { Card, CardContent, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useCustomerSatisfaction } from '../hooks/useCustomerSatisfaction'
import { defaultDateRange } from '../dateRange'

const { from, to } = defaultDateRange()

export function CsatSummaryWidget() {
  const { t, i18n } = useTranslation()
  const { data, isPending, isError, error, refetch } = useCustomerSatisfaction(from, to)

  return (
    <Card>
      <CardContent>
        <Typography variant="subtitle1" gutterBottom>
          {t('pages.dashboard.widgets.csat.title')}
        </Typography>

        {isPending && <LoadingState />}
        {isError && (
          <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
        )}
        {!isPending && !isError && data && data.totalResponses === 0 && (
          <EmptyState
            title={t('pages.dashboard.widgets.noData', { defaultValue: 'No data for this period' })}
          />
        )}
        {!isPending && !isError && data && data.totalResponses > 0 && (
          <Stack spacing={0.5}>
            <Typography variant="h4">
              {new Intl.NumberFormat(i18n.language, { maximumFractionDigits: 1 }).format(
                data.averageRating,
              )}{' '}
              / 5
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t('reports.csat.responses', {
                defaultValue: '{{count}} responses (last 30 days)',
                count: data.totalResponses,
              })}
            </Typography>
          </Stack>
        )}
      </CardContent>
    </Card>
  )
}
