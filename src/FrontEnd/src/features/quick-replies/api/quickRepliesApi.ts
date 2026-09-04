import { apiClient } from '@/lib/api/apiClient'
import type {
  CreateQuickReplyInput,
  QuickReplyTemplate,
  UpdateQuickReplyInput,
} from '../types/quickReply'

export async function listQuickReplies(search?: string): Promise<QuickReplyTemplate[]> {
  const response = await apiClient.get('/api/quick-replies', {
    params: search ? { search } : undefined,
  })
  return response.data
}

export async function getQuickReply(id: string): Promise<QuickReplyTemplate> {
  const response = await apiClient.get(`/api/quick-replies/${id}`)
  return response.data
}

export async function createQuickReply(input: CreateQuickReplyInput): Promise<QuickReplyTemplate> {
  const response = await apiClient.post('/api/quick-replies', input)
  return response.data
}

export async function updateQuickReply(
  id: string,
  input: UpdateQuickReplyInput,
): Promise<QuickReplyTemplate> {
  const response = await apiClient.put(`/api/quick-replies/${id}`, input)
  return response.data
}

export async function deleteQuickReply(id: string): Promise<void> {
  await apiClient.delete(`/api/quick-replies/${id}`)
}
