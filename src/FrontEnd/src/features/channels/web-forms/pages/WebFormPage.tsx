import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Container, Paper, Stack, TextField, Typography } from '@mui/material'
import { apiClient } from '@/lib/api/apiClient'
import { normalizeApiError } from '@/lib/api/normalizeApiError'

interface SubmitResult {
  ticketId: string
  trackingCode: string
}

/**
 * Public, unauthenticated support-request form (F03 web-forms-channel).
 * Registered under PublicRoute (not ProtectedRoute) - no login required.
 */
export function WebFormPage() {
  const { t } = useTranslation()

  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [subject, setSubject] = useState('')
  const [message, setMessage] = useState('')
  const [honeypot, setHoneypot] = useState('')
  const [submitting, setSubmitting] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [rateLimited, setRateLimited] = useState(false)
  const [result, setResult] = useState<SubmitResult | null>(null)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)
    setRateLimited(false)
    setSubmitting(true)

    try {
      const response = await apiClient.post('/api/v1/channels/web-forms/submissions', {
        submitterName: name,
        submitterEmail: email,
        subject,
        message,
        honeypot: honeypot || undefined,
      })
      setResult(response.data)
    } catch (err) {
      const apiError = normalizeApiError(err)
      if (apiError.status === 429) {
        setRateLimited(true)
      } else {
        setError(
          apiError.validationErrors?.SubmitterEmail?.[0] ??
            apiError.validationErrors?.Subject?.[0] ??
            apiError.validationErrors?.Message?.[0] ??
            apiError.detail ??
            t('webForms.submitFailed'),
        )
      }
    } finally {
      setSubmitting(false)
    }
  }

  if (result) {
    return (
      <Container maxWidth="sm" sx={{ py: 6 }}>
        <Paper sx={{ p: 4 }}>
          <Typography variant="h5" gutterBottom>
            {t('webForms.successTitle')}
          </Typography>
          <Typography variant="body1">
            {t('webForms.trackingCodeLabel')}: <strong>{result.trackingCode}</strong>
          </Typography>
        </Paper>
      </Container>
    )
  }

  return (
    <Container maxWidth="sm" sx={{ py: 6 }}>
      <Paper sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          {t('webForms.title')}
        </Typography>

        <Box component="form" onSubmit={handleSubmit}>
          <Stack spacing={2}>
            {error && <Alert severity="error">{error}</Alert>}
            {rateLimited && <Alert severity="warning">{t('webForms.rateLimited')}</Alert>}

            <TextField
              label={t('webForms.name')}
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              disabled={submitting}
            />
            <TextField
              label={t('webForms.email')}
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              disabled={submitting}
            />
            <TextField
              label={t('webForms.subject')}
              value={subject}
              onChange={(e) => setSubject(e.target.value)}
              required
              disabled={submitting}
            />
            <TextField
              label={t('webForms.message')}
              value={message}
              onChange={(e) => setMessage(e.target.value)}
              required
              multiline
              minRows={4}
              disabled={submitting}
            />

            {/* Honeypot: hidden from real users; a bot filling it marks the submission as spam. */}
            <TextField
              label="Company"
              value={honeypot}
              onChange={(e) => setHoneypot(e.target.value)}
              sx={{ position: 'absolute', left: '-9999px', width: 1, height: 1 }}
              tabIndex={-1}
              aria-hidden="true"
              autoComplete="off"
            />

            <Button type="submit" variant="contained" disabled={submitting}>
              {t('webForms.submit')}
            </Button>
          </Stack>
        </Box>
      </Paper>
    </Container>
  )
}
