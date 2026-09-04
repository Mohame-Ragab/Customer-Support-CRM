import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Container, Paper, Stack, TextField, Typography } from '@mui/material'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useSubmitPortalTicket } from '@/features/customer-portal/tickets/hooks'

export function SubmitTicketPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const mutation = useSubmitPortalTicket()

  const [subject, setSubject] = useState('')
  const [description, setDescription] = useState('')
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = () => {
    if (!subject.trim() || !description.trim()) return
    setError(null)
    mutation.mutate(
      { subject: subject.trim(), description: description.trim() },
      {
        onSuccess: (ticket) => navigate(`/portal/tickets/${ticket.id}`),
        onError: (err) => {
          const apiError = normalizeApiError(err)
          setError(
            apiError.validationErrors?.Subject?.[0] ??
              apiError.validationErrors?.Description?.[0] ??
              apiError.detail ??
              t('errors.unexpected'),
          )
        },
      },
    )
  }

  return (
    <Container maxWidth="sm" sx={{ py: 4 }}>
      <Paper sx={{ p: 4 }}>
        <Typography variant="h4" component="h1" gutterBottom>
          {t('portal.submitTicket.title')}
        </Typography>

        <Stack spacing={2}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label={t('portal.submitTicket.subject')}
            value={subject}
            onChange={(e) => setSubject(e.target.value)}
            disabled={mutation.isPending}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />
          <TextField
            label={t('portal.submitTicket.description')}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            disabled={mutation.isPending}
            multiline
            minRows={6}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />
          <Box>
            <Button
              variant="contained"
              onClick={handleSubmit}
              disabled={mutation.isPending || !subject.trim() || !description.trim()}
            >
              {t('portal.submitTicket.submit')}
            </Button>
          </Box>
        </Stack>
      </Paper>
    </Container>
  )
}
