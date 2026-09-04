import { apiClient } from '@/lib/api/apiClient'
import type { ChatMessage, ChatSession } from '../types'

export async function startChatSession(): Promise<ChatSession> {
  const response = await apiClient.post('/api/chat/sessions')
  return response.data
}

export async function getChatMessages(sessionId: string): Promise<ChatMessage[]> {
  const response = await apiClient.get(`/api/chat/sessions/${sessionId}/messages`)
  return response.data
}

export async function getOpenChatSessions(): Promise<ChatSession[]> {
  const response = await apiClient.get('/api/chat/sessions/open')
  return response.data
}
