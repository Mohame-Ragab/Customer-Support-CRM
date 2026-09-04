import { useRef, useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Alert, Box, Button, Snackbar, Stack, TextField } from '@mui/material'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { QuickReplyPickerButton } from '@/features/quick-replies/components/QuickReplyPickerButton'
import { insertQuickReplyIntoTextarea } from '@/features/quick-replies/hooks/useInsertQuickReply'
import { useSendEmailReply } from './useTicketMessages'

interface EmailReplyComposerProps {
  ticketId: string
  ticketSubject: string
}

export function EmailReplyComposer({ ticketId, ticketSubject }: EmailReplyComposerProps) {
  const { t } = useTranslation()
  const mutation = useSendEmailReply(ticketId)

  const [subject, setSubject] = useState(`Re: ${ticketSubject}`)
  const [body, setBody] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [showSuccess, setShowSuccess] = useState(false)
  const bodyFieldRef = useRef<HTMLTextAreaElement | null>(null)

  const handlePickQuickReply = (quickReplyBody: string) => {
    if (bodyFieldRef.current) {
      insertQuickReplyIntoTextarea(bodyFieldRef.current, quickReplyBody)
    } else {
      setBody((prev) => (prev ? `${prev}\n${quickReplyBody}` : quickReplyBody))
    }
  }

  const handleSend = () => {
    setError(null)
    mutation.mutate(
      { subject, bodyText: body },
      {
        onSuccess: () => {
          setBody('')
          setShowSuccess(true)
        },
        onError: (err) => {
          const apiError = normalizeApiError(err)
          setError(
            apiError.validationErrors?.Subject?.[0] ??
              apiError.validationErrors?.BodyText?.[0] ??
              apiError.detail ??
              t('emailChannel.sendFailed'),
          )
        },
      },
    )
  }

  return (
    <Box>
      <Stack spacing={2}>
        {error && <Alert severity="error">{error}</Alert>}

        <TextField
          label={t('emailChannel.subject')}
          value={subject}
          onChange={(e) => setSubject(e.target.value)}
          disabled={mutation.isPending}
          fullWidth
        />

        <TextField
          label={t('emailChannel.body')}
          value={body}
          onChange={(e) => setBody(e.target.value)}
          disabled={mutation.isPending}
          multiline
          minRows={4}
          fullWidth
          inputRef={bodyFieldRef}
        />

        <Box>
          <QuickReplyPickerButton onPick={handlePickQuickReply} />
        </Box>

        <Box>
          <Button
            variant="contained"
            disabled={mutation.isPending || !subject.trim() || !body.trim()}
            onClick={handleSend}
          >
            {t('emailChannel.send')}
          </Button>
        </Box>
      </Stack>

      <Snackbar open={showSuccess} autoHideDuration={4000} onClose={() => setShowSuccess(false)}>
        <Alert severity="success" onClose={() => setShowSuccess(false)}>
          {t('emailChannel.sent')}
        </Alert>
      </Snackbar>
    </Box>
  )
}
