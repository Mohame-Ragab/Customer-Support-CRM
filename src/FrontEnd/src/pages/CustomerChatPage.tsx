import { useTranslation } from 'react-i18next'
import { Container, Typography } from '@mui/material'
import { CustomerChatPanel } from '@/features/chat/components/CustomerChatPanel'

export function CustomerChatPage() {
  const { t } = useTranslation()

  return (
    <Container
      maxWidth="sm"
      sx={{ py: 4, height: '80vh', display: 'flex', flexDirection: 'column' }}
    >
      <Typography variant="h4" component="h1" gutterBottom>
        {t('chat.startChat')}
      </Typography>
      <CustomerChatPanel />
    </Container>
  )
}
