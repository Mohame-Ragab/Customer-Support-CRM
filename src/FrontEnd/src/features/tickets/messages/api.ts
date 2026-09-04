import { apiClient } from '@/lib/api/apiClient'
import type { TicketMessage } from './types'

export async function listTicketMessages(ticketId: string): Promise<TicketMessage[]> {
  const response = await apiClient.get(`/api/tickets/${ticketId}/messages`)
  return response.data
}

export interface SendEmailReplyPayload {
  subject: string
  bodyText: string
  bodyHtml?: string | null
}

export async function sendEmailReply(
  ticketId: string,
  payload: SendEmailReplyPayload,
): Promise<TicketMessage> {
  const response = await apiClient.post(`/api/tickets/${ticketId}/messages/email`, payload)
  return response.data
}
