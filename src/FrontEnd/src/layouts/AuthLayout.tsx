import Box from '@mui/material/Box'
import Container from '@mui/material/Container'
import Paper from '@mui/material/Paper'
import Typography from '@mui/material/Typography'
import { useTranslation } from 'react-i18next'
import { Outlet } from 'react-router-dom'

import { appConfig } from '@/config/appConfig'

/** Shell for unauthenticated routes (login, ...): centered card, no app chrome. */
export function AuthLayout() {
  const { t } = useTranslation()

  return (
    <Box
      sx={{
        minHeight: '100vh',
        display: 'flex',
        alignItems: 'center',
        justifyContent: 'center',
        bgcolor: 'background.default',
      }}
    >
      <Container maxWidth="sm" sx={{ py: { xs: 3, sm: 6 }, px: { xs: 2, sm: 3 } }}>
        <Paper elevation={2} sx={{ p: 4, width: '100%', maxWidth: 420, mx: 'auto' }}>
          <Typography variant="h5" component="h1" align="center" gutterBottom>
            {t('app.name', { defaultValue: appConfig.appName })}
          </Typography>
          <Outlet />
        </Paper>
      </Container>
    </Box>
  )
}
