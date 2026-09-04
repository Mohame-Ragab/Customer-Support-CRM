import Alert from '@mui/material/Alert'
import AlertTitle from '@mui/material/AlertTitle'
import Box from '@mui/material/Box'
import Button from '@mui/material/Button'
import { useTranslation } from 'react-i18next'

interface ErrorStateProps {
  title?: string
  message?: string
  onRetry?: () => void
}

/**
 * Generic error placeholder driven by a normalized ApiError's title/detail
 * (see types/api.ts) or a plain message. No feature-specific variant belongs
 * here; feature errors are rendered by passing their own translated message in.
 */
export function ErrorState({ title, message, onRetry }: ErrorStateProps) {
  const { t } = useTranslation()

  return (
    <Box sx={{ p: 2 }}>
      <Alert
        severity="error"
        action={
          onRetry ? (
            <Button color="inherit" size="small" onClick={onRetry}>
              {t('actions.retry', { defaultValue: 'Retry' })}
            </Button>
          ) : undefined
        }
      >
        <AlertTitle>{title ?? t('errors.unexpected')}</AlertTitle>
        {message}
      </Alert>
    </Box>
  )
}
