import { apiClient } from '@/lib/api/apiClient'
import type { Ticket, TicketCategory, TicketPriority } from '../types'

export async function fetchTicketCategories(): Promise<TicketCategory[]> {
  const response = await apiClient.get('/api/ticket-categories')
  return response.data
}

export interface SetTicketClassificationInput {
  categoryId?: string | null
  priority?: TicketPriority | null
}

export async function setTicketClassification(
  ticketId: string,
  body: SetTicketClassificationInput,
): Promise<Ticket> {
  const response = await apiClient.patch(`/api/tickets/${ticketId}/classification`, body)
  return response.data
}
