import { useMutation, useQueryClient } from '@tanstack/react-query'
import { createTask, deleteTask, updateTask } from '../api/agentTasksApi'
import type { CreateAgentTaskInput, UpdateAgentTaskInput } from '../types/agentTask'

export function useAgentTaskMutations() {
  const queryClient = useQueryClient()
  const invalidate = () => queryClient.invalidateQueries({ queryKey: ['agent-tasks'] })

  const createMutation = useMutation({
    mutationFn: (input: CreateAgentTaskInput) => createTask(input),
    onSuccess: invalidate,
  })

  const updateMutation = useMutation({
    mutationFn: ({ id, input }: { id: string; input: UpdateAgentTaskInput }) =>
      updateTask(id, input),
    onSuccess: invalidate,
  })

  const deleteMutation = useMutation({
    mutationFn: (id: string) => deleteTask(id),
    onSuccess: invalidate,
  })

  return { createMutation, updateMutation, deleteMutation }
}
