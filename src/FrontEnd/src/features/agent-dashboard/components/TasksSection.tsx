import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Alert,
  Box,
  Button,
  FormControlLabel,
  Snackbar,
  Stack,
  Switch,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useAgentTasks } from '../hooks/useAgentTasks'
import { useAgentTaskMutations } from '../hooks/useAgentTaskMutations'
import type { AgentTask } from '../types/agentTask'
import { TaskList } from './TaskList'
import { TaskFormDialog } from './TaskFormDialog'

export function TasksSection() {
  const { t } = useTranslation()
  const [includeCompleted, setIncludeCompleted] = useState(false)
  const { data: tasks, isPending, isError, error, refetch } = useAgentTasks(includeCompleted)
  const { createMutation, updateMutation, deleteMutation } = useAgentTaskMutations()

  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingTask, setEditingTask] = useState<AgentTask | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)

  const openCreateDialog = () => {
    setEditingTask(null)
    setDialogOpen(true)
  }

  const openEditDialog = (task: AgentTask) => {
    setEditingTask(task)
    setDialogOpen(true)
  }

  const handleSubmit = (values: { description: string; dueAt: string }) => {
    const mutation = editingTask
      ? updateMutation.mutateAsync({
          id: editingTask.id,
          input: {
            ...values,
            isCompleted: editingTask.isCompleted,
            ticketId: editingTask.ticketId,
          },
        })
      : createMutation.mutateAsync(values)

    mutation
      .then(() => setDialogOpen(false))
      .catch((err) => setErrorMessage(normalizeApiError(err).detail ?? t('errors.unexpected')))
  }

  const handleToggleComplete = (task: AgentTask) => {
    updateMutation
      .mutateAsync({
        id: task.id,
        input: {
          description: task.description,
          dueAt: task.dueAt,
          isCompleted: !task.isCompleted,
          ticketId: task.ticketId,
        },
      })
      .catch((err) => setErrorMessage(normalizeApiError(err).detail ?? t('errors.unexpected')))
  }

  const handleDelete = (task: AgentTask) => {
    deleteMutation
      .mutateAsync(task.id)
      .catch((err) => setErrorMessage(normalizeApiError(err).detail ?? t('errors.unexpected')))
  }

  return (
    <Box>
      <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', mb: 1 }}>
        <Typography variant="subtitle1">{t('agentDashboard.tasks.title')}</Typography>
        <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
          <FormControlLabel
            control={
              <Switch
                checked={includeCompleted}
                onChange={(e) => setIncludeCompleted(e.target.checked)}
                size="small"
              />
            }
            label={t('agentDashboard.tasks.completed')}
          />
          <Button size="small" variant="contained" onClick={openCreateDialog}>
            {t('agentDashboard.tasks.new')}
          </Button>
        </Stack>
      </Stack>

      {isPending && <LoadingState />}

      {isError && (
        <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
      )}

      {!isPending && !isError && (tasks?.length ?? 0) === 0 && (
        <EmptyState title={t('agentDashboard.tasks.empty')} />
      )}

      {!isPending && !isError && (tasks?.length ?? 0) > 0 && (
        <TaskList
          tasks={tasks ?? []}
          onToggleComplete={handleToggleComplete}
          onEdit={openEditDialog}
          onDelete={handleDelete}
        />
      )}

      <TaskFormDialog
        open={dialogOpen}
        task={editingTask}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
        submitting={createMutation.isPending || updateMutation.isPending}
      />

      <Snackbar open={!!errorMessage} autoHideDuration={5000} onClose={() => setErrorMessage(null)}>
        <Alert severity="error" onClose={() => setErrorMessage(null)}>
          {errorMessage}
        </Alert>
      </Snackbar>
    </Box>
  )
}
