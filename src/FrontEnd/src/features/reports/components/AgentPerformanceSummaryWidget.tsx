import { Link as RouterLink } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAgentPerformance } from '../hooks/useAgentPerformance'
import { defaultDateRange } from '../dateRange'

const { from, to } = defaultDateRange()

export function AgentPerformanceSummaryWidget() {
  const { t, i18n } = useTranslation()
  const { data, isPending, isError, error, refetch } = useAgentPerformance({ from, to })

  const topAgent = data?.rows[0]

  return (
    <Card>
      <CardContent>
        <Typography variant="subtitle1" gutterBottom>
          {t('pages.dashboard.widgets.agents.title')}
        </Typography>

        {isPending && <LoadingState />}
        {isError && (
          <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
        )}
        {!isPending && !isError && (data?.rows.length ?? 0) === 0 && (
          <EmptyState
            title={t('pages.dashboard.widgets.noData', { defaultValue: 'No data for this period' })}
          />
        )}
        {!isPending && !isError && topAgent && (
          <Stack spacing={0.5}>
            <Typography variant="h4">
              {new Intl.NumberFormat(i18n.language).format(data?.rows.length ?? 0)}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              {t('reports.agentPerformance.agentsActive', {
                defaultValue: 'Agents active (last 30 days)',
              })}
            </Typography>
          </Stack>
        )}

        <Button component={RouterLink} to="/reports/agent-performance" size="small" sx={{ mt: 1 }}>
          {t('pages.dashboard.widgets.agents.viewFull')}
        </Button>
      </CardContent>
    </Card>
  )
}
