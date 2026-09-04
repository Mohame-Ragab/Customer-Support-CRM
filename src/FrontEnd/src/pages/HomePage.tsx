import Typography from '@mui/material/Typography'
import { useTranslation } from 'react-i18next'

/**
 * Architecture-verification placeholder for the authenticated "/" route.
 * Replaced once a real dashboard/first feature is implemented.
 */
export function HomePage() {
  const { t } = useTranslation()

  return (
    <>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('pages.home.title')}
      </Typography>
      <Typography color="text.secondary">{t('pages.home.body')}</Typography>
    </>
  )
}
