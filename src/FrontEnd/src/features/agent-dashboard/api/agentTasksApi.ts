import { apiClient } from '@/lib/api/apiClient'
import type { AgentTask, CreateAgentTaskInput, UpdateAgentTaskInput } from '../types/agentTask'

export async function listMyTasks(includeCompleted: boolean): Promise<AgentTask[]> {
  const response = await apiClient.get('/api/agent-tasks', { params: { includeCompleted } })
  return response.data
}

export async function createTask(input: CreateAgentTaskInput): Promise<AgentTask> {
  const response = await apiClient.post('/api/agent-tasks', input)
  return response.data
}

export async function updateTask(id: string, input: UpdateAgentTaskInput): Promise<AgentTask> {
  const response = await apiClient.put(`/api/agent-tasks/${id}`, input)
  return response.data
}

export async function deleteTask(id: string): Promise<void> {
  await apiClient.delete(`/api/agent-tasks/${id}`)
}
