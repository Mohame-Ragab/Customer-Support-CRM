import { List } from '@mui/material'
import type { AgentTask } from '../types/agentTask'
import { TaskListItem } from './TaskListItem'

interface TaskListProps {
  tasks: AgentTask[]
  onToggleComplete: (task: AgentTask) => void
  onEdit: (task: AgentTask) => void
  onDelete: (task: AgentTask) => void
}

export function TaskList({ tasks, onToggleComplete, onEdit, onDelete }: TaskListProps) {
  return (
    <List>
      {tasks.map((task) => (
        <TaskListItem
          key={task.id}
          task={task}
          onToggleComplete={onToggleComplete}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </List>
  )
}
