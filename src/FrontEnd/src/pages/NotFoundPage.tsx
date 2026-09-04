import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import Typography from '@mui/material/Typography'
import { useTranslation } from 'react-i18next'
import { Link as RouterLink } from 'react-router-dom'

export function NotFoundPage() {
  const { t } = useTranslation()

  return (
    <Box sx={{ textAlign: 'center', p: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('pages.notFound.title')}
      </Typography>
      <Typography color="text.secondary" gutterBottom>
        {t('pages.notFound.body')}
      </Typography>
      <Button component={RouterLink} to="/" variant="contained" sx={{ mt: 2 }}>
        {t('actions.goHome')}
      </Button>
    </Box>
  )
}
