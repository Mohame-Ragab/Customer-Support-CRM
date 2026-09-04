import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from '@mui/material'
import type { QuickReplyTemplate } from '../types/quickReply'

interface QuickReplyFormProps {
  open: boolean
  template: QuickReplyTemplate | null
  onClose: () => void
  onSubmit: (values: { name: string; body: string }) => void
  submitting: boolean
  error: string | null
}

/**
 * Field state lives in this inner component, keyed by template identity in
 * QuickReplyForm below, so switching between "new" and "edit template X"
 * re-initializes from props via useState's lazy initializer on remount - no
 * effect-based reset (avoids react-hooks/set-state-in-effect).
 */
function QuickReplyFormContent({
  template, onClose, onSubmit, submitting, error,
}: Omit<QuickReplyFormProps, 'open'>) {
  const { t } = useTranslation()
  const [name, setName] = useState(template?.name ?? '')
  const [body, setBody] = useState(template?.body ?? '')

  const nameTooLong = Array.from(name).length > 100
  const bodyTooLong = Array.from(body).length > 4000

  return (
    <>
      <DialogTitle>{template ? t('quickReplies.edit') : t('quickReplies.create')}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {error && <Stack sx={{ color: 'error.main' }}>{error}</Stack>}
          <TextField
            label={t('quickReplies.name')}
            value={name}
            onChange={(e) => setName(e.target.value)}
            error={nameTooLong}
            helperText={nameTooLong ? t('quickReplies.errors.nameTooLong') : undefined}
            disabled={submitting}
            fullWidth
          />
          <TextField
            label={t('quickReplies.body')}
            value={body}
            onChange={(e) => setBody(e.target.value)}
            error={bodyTooLong}
            helperText={bodyTooLong ? t('quickReplies.errors.bodyTooLong') : undefined}
            multiline
            minRows={6}
            disabled={submitting}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={submitting}>
          {t('agentDashboard.tasks.cancel', { defaultValue: 'Cancel' })}
        </Button>
        <Button
          variant="contained"
          disabled={submitting || !name.trim() || !body.trim() || nameTooLong || bodyTooLong}
          onClick={() => onSubmit({ name: name.trim(), body: body.trim() })}
        >
          {t('agentDashboard.tasks.save', { defaultValue: 'Save' })}
        </Button>
      </DialogActions>
    </>
  )
}

export function QuickReplyForm({ open, template, onClose, onSubmit, submitting, error }: QuickReplyFormProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      {open && (
        <QuickReplyFormContent
          key={template?.id ?? 'new'}
          template={template}
          onClose={onClose}
          onSubmit={onSubmit}
          submitting={submitting}
          error={error}
        />
      )}
    </Dialog>
  )
}
