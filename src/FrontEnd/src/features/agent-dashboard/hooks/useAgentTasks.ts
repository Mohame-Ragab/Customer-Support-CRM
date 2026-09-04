import { useQuery } from '@tanstack/react-query'
import { listMyTasks } from '../api/agentTasksApi'

export function useAgentTasks(includeCompleted: boolean) {
  return useQuery({
    queryKey: ['agent-tasks', { includeCompleted }],
    queryFn: () => listMyTasks(includeCompleted),
  })
}
