import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Rating, TextField, Typography } from '@mui/material'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useSubmitFeedback } from './hooks'

interface FeedbackFormProps {
  ticketId: string
  onSubmitted?: () => void
}

export function FeedbackForm({ ticketId, onSubmitted }: FeedbackFormProps) {
  const { t } = useTranslation()
  const mutation = useSubmitFeedback(ticketId)
  const [rating, setRating] = useState<number | null>(null)
  const [comment, setComment] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [submitted, setSubmitted] = useState(false)

  const handleSubmit = () => {
    if (!rating) return
    setError(null)
    mutation.mutate(
      { rating, comment: comment.trim() || undefined },
      {
        onSuccess: () => {
          setSubmitted(true)
          onSubmitted?.()
        },
        onError: (err) => {
          const apiError = normalizeApiError(err)
          if (apiError.status === 409) {
            setError(
              t('feedback.alreadySubmitted', {
                defaultValue: 'Feedback has already been submitted for this ticket.',
              }),
            )
          } else {
            setError(
              apiError.validationErrors?.Rating?.[0] ?? apiError.detail ?? t('errors.unexpected'),
            )
          }
        },
      },
    )
  }

  if (submitted) {
    return (
      <Alert severity="success">
        {t('feedback.thankYou', { defaultValue: 'Thank you for your feedback!' })}
      </Alert>
    )
  }

  return (
    <Box>
      <Typography variant="subtitle1" gutterBottom>
        {t('feedback.title')}
      </Typography>
      {error && (
        <Alert severity="error" sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}
      <Rating
        value={rating}
        onChange={(_, value) => setRating(value)}
        size="large"
        disabled={mutation.isPending}
      />
      <TextField
        fullWidth
        multiline
        minRows={3}
        sx={{ mt: 2 }}
        placeholder={t('feedback.commentPlaceholder', { defaultValue: 'Tell us more (optional)…' })}
        value={comment}
        onChange={(e) => setComment(e.target.value)}
        disabled={mutation.isPending}
        slotProps={{ htmlInput: { dir: 'auto' } }}
      />
      <Box sx={{ mt: 2 }}>
        <Button variant="contained" onClick={handleSubmit} disabled={!rating || mutation.isPending}>
          {t('feedback.submit')}
        </Button>
      </Box>
    </Box>
  )
}
