import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, TextField } from '@mui/material'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAddTicketInternalComment } from '../hooks/useTicketInternalComments'

interface InternalCommentComposerProps {
  ticketId: string
}

export function InternalCommentComposer({ ticketId }: InternalCommentComposerProps) {
  const { t } = useTranslation()
  const mutation = useAddTicketInternalComment(ticketId)
  const [body, setBody] = useState('')
  const [error, setError] = useState<string | null>(null)

  const handleSubmit = () => {
    const trimmed = body.trim()
    if (!trimmed) return

    setError(null)
    mutation.mutate(trimmed, {
      onSuccess: () => setBody(''),
      onError: (err) => {
        const apiError = normalizeApiError(err)
        setError(
          apiError.validationErrors?.Body?.[0] ??
            apiError.detail ??
            t('ticketInternalComments.errorSubmit'),
        )
      },
    })
  }

  return (
    <Box>
      {error && (
        <Alert severity="error" sx={{ mb: 1 }}>
          {error}
        </Alert>
      )}
      <TextField
        fullWidth
        multiline
        minRows={3}
        placeholder={t('ticketInternalComments.composerPlaceholder')}
        value={body}
        onChange={(e) => setBody(e.target.value)}
        disabled={mutation.isPending}
        slotProps={{ htmlInput: { dir: 'auto' } }}
      />
      <Box sx={{ mt: 1, textAlign: 'right' }}>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={mutation.isPending || !body.trim()}
        >
          {t('ticketInternalComments.submit')}
        </Button>
      </Box>
    </Box>
  )
}
