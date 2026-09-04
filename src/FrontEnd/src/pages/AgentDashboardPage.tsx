import { useTranslation } from 'react-i18next'
import { Card, CardContent, Container, Stack, Typography } from '@mui/material'
import { TasksSection } from '@/features/agent-dashboard/components/TasksSection'
import { AssignedTicketsSection } from '@/features/tickets/components/AssignedTicketsSection'

/**
 * Agent's personal dashboard (F04 agent-dashboard): tasks/reminders and the
 * agent's own assigned-ticket queue, in one place.
 */
export function AgentDashboardPage() {
  const { t } = useTranslation()

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('agentDashboard.title')}
      </Typography>

      <Stack spacing={3}>
        <Card>
          <CardContent>
            <TasksSection />
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Typography variant="subtitle1" gutterBottom>
              {t('agentDashboard.tickets.title', { defaultValue: 'My Assigned Tickets' })}
            </Typography>
            <AssignedTicketsSection />
          </CardContent>
        </Card>
      </Stack>
    </Container>
  )
}
