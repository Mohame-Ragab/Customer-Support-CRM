import { useTranslation } from 'react-i18next'
import { Container, Typography } from '@mui/material'
import { AgentChatQueue } from '@/features/chat/components/AgentChatQueue'

export function AgentChatQueuePage() {
  const { t } = useTranslation()

  return (
    <Container
      maxWidth="lg"
      sx={{ py: 4, height: '80vh', display: 'flex', flexDirection: 'column' }}
    >
      <Typography variant="h4" component="h1" gutterBottom>
        {t('nav.chatQueue', { defaultValue: 'Chat queue' })}
      </Typography>
      <AgentChatQueue />
    </Container>
  )
}
