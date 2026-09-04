import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  MenuItem,
  Stack,
  Switch,
  TextField,
} from '@mui/material'
import type { KnowledgeBaseContent } from '../types'
import { ContentType } from '../types'

export interface ContentFormValues {
  title: string
  body: string
  summary: string
  type: ContentType
  language: string
  isPublished: boolean
}

interface ContentFormDialogProps {
  open: boolean
  content: KnowledgeBaseContent | null
  onClose: () => void
  onSubmit: (values: ContentFormValues) => void
  submitting: boolean
  error: string | null
}

const TYPE_OPTIONS: ContentType[] = [
  ContentType.Faq,
  ContentType.Article,
  ContentType.SolutionGuide,
]

const TYPE_LABEL_KEYS: Record<ContentType, string> = {
  [ContentType.Faq]: 'knowledgeBase.type.faq',
  [ContentType.Article]: 'knowledgeBase.type.article',
  [ContentType.SolutionGuide]: 'knowledgeBase.type.solutionGuide',
}

/**
 * Field state lives in this inner component, keyed by content identity in
 * ContentFormDialog below, so switching between "new" and "edit item X"
 * re-initializes from props via useState's lazy initializer on remount - no
 * effect-based reset (avoids react-hooks/set-state-in-effect; same pattern as
 * F04's TaskFormDialog/QuickReplyForm).
 */
function ContentFormDialogContent({
  content,
  onClose,
  onSubmit,
  submitting,
  error,
}: Omit<ContentFormDialogProps, 'open'>) {
  const { t } = useTranslation()
  const [title, setTitle] = useState(content?.title ?? '')
  const [body, setBody] = useState(content?.body ?? '')
  const [summary, setSummary] = useState(content?.summary ?? '')
  const [type, setType] = useState<ContentType>(content?.type ?? ContentType.Faq)
  const [language, setLanguage] = useState(content?.language ?? 'en')
  const [isPublished, setIsPublished] = useState(content?.isPublished ?? true)

  const handleSubmit = () => {
    if (!title.trim() || !body.trim()) return
    onSubmit({ title: title.trim(), body, summary: summary.trim(), type, language, isPublished })
  }

  return (
    <>
      <DialogTitle>
        {content
          ? t('knowledgeBase.edit')
          : t('knowledgeBase.create', { defaultValue: 'New content' })}
      </DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {error && <Stack sx={{ color: 'error.main' }}>{error}</Stack>}

          <TextField
            label={t('knowledgeBase.form.title', { defaultValue: 'Title' })}
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            disabled={submitting}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />

          <Stack direction="row" spacing={2}>
            <TextField
              select
              label={t('knowledgeBase.form.type', { defaultValue: 'Type' })}
              value={type}
              onChange={(e) => setType(Number(e.target.value) as ContentType)}
              disabled={submitting}
              sx={{ flex: 1 }}
            >
              {TYPE_OPTIONS.map((option) => (
                <MenuItem key={option} value={option}>
                  {t(TYPE_LABEL_KEYS[option])}
                </MenuItem>
              ))}
            </TextField>

            <TextField
              select
              label={t('knowledgeBase.form.language', { defaultValue: 'Language' })}
              value={language}
              onChange={(e) => setLanguage(e.target.value)}
              disabled={submitting}
              sx={{ flex: 1 }}
            >
              <MenuItem value="en">{t('knowledgeBase.language.en')}</MenuItem>
              <MenuItem value="ar">{t('knowledgeBase.language.ar')}</MenuItem>
            </TextField>
          </Stack>

          <TextField
            label={t('knowledgeBase.form.summary', { defaultValue: 'Summary (optional)' })}
            value={summary}
            onChange={(e) => setSummary(e.target.value)}
            disabled={submitting}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />

          <TextField
            label={t('knowledgeBase.form.body', { defaultValue: 'Body' })}
            value={body}
            onChange={(e) => setBody(e.target.value)}
            disabled={submitting}
            multiline
            minRows={8}
            fullWidth
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />

          <FormControlLabel
            control={
              <Switch
                checked={isPublished}
                onChange={(e) => setIsPublished(e.target.checked)}
                disabled={submitting}
              />
            }
            label={t('knowledgeBase.published')}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={submitting}>
          {t('agentDashboard.tasks.cancel', { defaultValue: 'Cancel' })}
        </Button>
        <Button
          variant="contained"
          onClick={handleSubmit}
          disabled={submitting || !title.trim() || !body.trim()}
        >
          {t('agentDashboard.tasks.save', { defaultValue: 'Save' })}
        </Button>
      </DialogActions>
    </>
  )
}

export function ContentFormDialog({
  open,
  content,
  onClose,
  onSubmit,
  submitting,
  error,
}: ContentFormDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="md">
      {open && (
        <ContentFormDialogContent
          key={content?.id ?? 'new'}
          content={content}
          onClose={onClose}
          onSubmit={onSubmit}
          submitting={submitting}
          error={error}
        />
      )}
    </Dialog>
  )
}
