import { apiClient } from '@/lib/api/apiClient'
import type { Ticket } from '@/features/tickets/types'
import type { PagedResult } from '@/features/tickets/api/ticketsApi'

export interface SubmitPortalTicketInput {
  subject: string
  description: string
}

export async function submitPortalTicket(input: SubmitPortalTicketInput): Promise<Ticket> {
  const response = await apiClient.post('/api/customer-portal/tickets', input)
  return response.data
}

export interface ListMyPortalTicketsParams {
  page?: number
  pageSize?: number
}

export async function listMyPortalTickets(
  params: ListMyPortalTicketsParams = {},
): Promise<PagedResult<Ticket>> {
  const response = await apiClient.get('/api/customer-portal/tickets', { params })
  return response.data
}

export async function getMyPortalTicket(id: string): Promise<Ticket> {
  const response = await apiClient.get(`/api/customer-portal/tickets/${id}`)
  return response.data
}
