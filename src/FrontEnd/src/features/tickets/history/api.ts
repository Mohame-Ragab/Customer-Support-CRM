import { apiClient } from '@/lib/api/apiClient'
import type { TicketHistoryEntry } from './types'

/** Matches the actual backend response shape: GET returns a plain array, not a wrapped envelope. */
export async function getTicketHistory(ticketId: string): Promise<TicketHistoryEntry[]> {
  const response = await apiClient.get(`/api/tickets/${ticketId}/history`)
  return response.data
}
