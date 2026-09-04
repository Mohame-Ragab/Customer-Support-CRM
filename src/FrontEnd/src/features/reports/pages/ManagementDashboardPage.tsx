import { useTranslation } from 'react-i18next'
import { Container, Grid, Typography } from '@mui/material'
import { TicketSummaryWidget } from '../components/TicketSummaryWidget'
import { AgentPerformanceSummaryWidget } from '../components/AgentPerformanceSummaryWidget'
import { CsatSummaryWidget } from '../components/CsatSummaryWidget'

// NOTE(sla-widget): the SLA Performance widget was removed - FR-047 is out of
// scope (see .squad/plans/reports-management/00-overview.md). This dashboard
// composes exactly three widgets: Ticket, Agent Performance, CSAT.
export function ManagementDashboardPage() {
  const { t } = useTranslation()

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('pages.dashboard.title')}
      </Typography>
      <Typography variant="body1" color="text.secondary" gutterBottom>
        {t('pages.dashboard.subtitle')}
      </Typography>

      <Grid container spacing={3} sx={{ mt: 1 }}>
        <Grid size={{ xs: 12, md: 4 }}>
          <TicketSummaryWidget />
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <AgentPerformanceSummaryWidget />
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <CsatSummaryWidget />
        </Grid>
      </Grid>
    </Container>
  )
}
