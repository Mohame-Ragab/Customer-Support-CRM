import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Button, Dialog, DialogActions, DialogContent, DialogTitle, Stack, TextField } from '@mui/material'
import type { AgentTask } from '../types/agentTask'

interface TaskFormDialogProps {
  open: boolean
  task: AgentTask | null
  onClose: () => void
  onSubmit: (values: { description: string; dueAt: string }) => void
  submitting: boolean
}

function toLocalInputValue(iso: string | null): string {
  if (!iso) return ''
  const date = new Date(iso)
  const pad = (n: number) => String(n).padStart(2, '0')
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`
}

/**
 * The form fields live in a separate inner component keyed by the dialog's
 * open/task identity (see TaskFormDialog below), so switching between "new
 * task" and "edit task X" re-initializes state from props via useState's
 * lazy initializer on remount - no effect-based reset needed (avoids
 * react-hooks/set-state-in-effect).
 */
function TaskFormDialogContent({ task, onClose, onSubmit, submitting }: Omit<TaskFormDialogProps, 'open'>) {
  const { t } = useTranslation()
  const [description, setDescription] = useState(task?.description ?? '')
  const [dueAt, setDueAt] = useState(toLocalInputValue(task?.dueAt ?? null))

  const handleSubmit = () => {
    if (!description.trim() || !dueAt) return
    onSubmit({ description: description.trim(), dueAt: new Date(dueAt).toISOString() })
  }

  return (
    <>
      <DialogTitle>
        {task ? t('agentDashboard.tasks.edit', { defaultValue: 'Edit task' }) : t('agentDashboard.tasks.new')}
      </DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label={t('agentDashboard.tasks.form.description')}
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            multiline
            minRows={3}
            fullWidth
            disabled={submitting}
            slotProps={{ htmlInput: { dir: 'auto' } }}
          />
          <TextField
            label={t('agentDashboard.tasks.form.dueAt')}
            type="datetime-local"
            value={dueAt}
            onChange={(e) => setDueAt(e.target.value)}
            fullWidth
            disabled={submitting}
            slotProps={{ inputLabel: { shrink: true } }}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={submitting}>
          {t('agentDashboard.tasks.cancel', { defaultValue: 'Cancel' })}
        </Button>
        <Button variant="contained" onClick={handleSubmit} disabled={submitting || !description.trim() || !dueAt}>
          {t('agentDashboard.tasks.save', { defaultValue: 'Save' })}
        </Button>
      </DialogActions>
    </>
  )
}

export function TaskFormDialog({ open, task, onClose, onSubmit, submitting }: TaskFormDialogProps) {
  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      {open && (
        <TaskFormDialogContent
          key={task?.id ?? 'new'}
          task={task}
          onClose={onClose}
          onSubmit={onSubmit}
          submitting={submitting}
        />
      )}
    </Dialog>
  )
}
