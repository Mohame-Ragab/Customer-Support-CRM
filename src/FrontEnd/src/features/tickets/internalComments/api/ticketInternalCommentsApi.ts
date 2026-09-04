import { apiClient } from '@/lib/api/apiClient'
import type { TicketInternalComment } from '../types'

export async function listInternalComments(ticketId: string): Promise<TicketInternalComment[]> {
  const response = await apiClient.get(`/api/tickets/${ticketId}/internal-comments`)
  return response.data
}

export async function addInternalComment(
  ticketId: string,
  body: string,
): Promise<TicketInternalComment> {
  const response = await apiClient.post(`/api/tickets/${ticketId}/internal-comments`, { body })
  return response.data
}
