import { useTranslation } from 'react-i18next'
import { Checkbox, Chip, IconButton, ListItem, ListItemText, Stack, Tooltip } from '@mui/material'
import EditIcon from '@mui/icons-material/Edit'
import DeleteIcon from '@mui/icons-material/Delete'
import type { AgentTask } from '../types/agentTask'

interface TaskListItemProps {
  task: AgentTask
  onToggleComplete: (task: AgentTask) => void
  onEdit: (task: AgentTask) => void
  onDelete: (task: AgentTask) => void
}

export function TaskListItem({ task, onToggleComplete, onEdit, onDelete }: TaskListItemProps) {
  const { t } = useTranslation()

  // Client-side fallback for time drift on a long-open tab; server's
  // isOverdue is authoritative at query time (see AgentTaskDto).
  const overdue = task.isOverdue || (!task.isCompleted && new Date(task.dueAt) < new Date())

  return (
    <ListItem
      divider
      secondaryAction={
        <Stack direction="row" spacing={0.5}>
          <Tooltip title={t('agentDashboard.tasks.edit', { defaultValue: 'Edit' })}>
            <IconButton edge="end" size="small" onClick={() => onEdit(task)}>
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title={t('agentDashboard.tasks.delete', { defaultValue: 'Delete' })}>
            <IconButton edge="end" size="small" onClick={() => onDelete(task)}>
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Stack>
      }
    >
      <Checkbox edge="start" checked={task.isCompleted} onChange={() => onToggleComplete(task)} />
      <ListItemText
        primary={<span dir="auto">{task.description}</span>}
        secondary={new Date(task.dueAt).toLocaleString()}
      />
      <Stack direction="row" spacing={1} sx={{ ml: 1 }}>
        {task.isCompleted && (
          <Chip size="small" color="success" label={t('agentDashboard.tasks.completed')} />
        )}
        {!task.isCompleted && overdue && (
          <Chip size="small" color="error" label={t('agentDashboard.tasks.overdue')} />
        )}
      </Stack>
    </ListItem>
  )
}
