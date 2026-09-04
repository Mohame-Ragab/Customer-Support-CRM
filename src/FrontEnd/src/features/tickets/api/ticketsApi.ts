import { apiClient } from '@/lib/api/apiClient'
import type { Ticket } from '../types'

export interface PagedResult<T> {
  items: T[]
  page: number
  pageSize: number
  totalCount: number
}

export async function fetchTicket(ticketId: string): Promise<Ticket> {
  const response = await apiClient.get(`/api/tickets/${ticketId}`)
  return response.data
}

export interface FetchTicketsParams {
  page?: number
  pageSize?: number
  /** F04 agent-dashboard/view-assigned-tickets: restrict to the caller's own assigned tickets. */
  assignedToMe?: boolean
}

export async function fetchTickets(params: FetchTicketsParams = {}): Promise<PagedResult<Ticket>> {
  const response = await apiClient.get('/api/tickets', { params })
  return response.data
}
