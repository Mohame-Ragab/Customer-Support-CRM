import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Container, Typography, Card, CardContent, Snackbar, Alert } from '@mui/material'
import { BrandingForm } from '@/features/branding/components/BrandingForm'

export function BrandingPage() {
  const { t } = useTranslation()
  const [showSuccess, setShowSuccess] = useState(false)

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('branding.title')}
      </Typography>

      <Card sx={{ mt: 3 }}>
        <CardContent>
          <BrandingForm onSaved={() => setShowSuccess(true)} />
        </CardContent>
      </Card>

      <Snackbar open={showSuccess} autoHideDuration={4000} onClose={() => setShowSuccess(false)}>
        <Alert severity="success" onClose={() => setShowSuccess(false)}>
          {t('branding.saved')}
        </Alert>
      </Snackbar>
    </Container>
  )
}
