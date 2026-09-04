export interface AgentTask {
  id: string
  description: string
  dueAt: string // ISO-8601 UTC
  isCompleted: boolean
  completedAt: string | null
  isOverdue: boolean
  ticketId: string | null
  createdAt: string
  updatedAt: string | null
}

export interface CreateAgentTaskInput {
  description: string
  dueAt: string
  ticketId?: string | null
}

export interface UpdateAgentTaskInput {
  description: string
  dueAt: string
  isCompleted: boolean
  ticketId?: string | null
}
