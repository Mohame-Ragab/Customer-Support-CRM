import { apiClient } from '@/lib/api/apiClient'
import type { CustomerNote } from './types'

export async function listCustomerNotes(customerId: string): Promise<CustomerNote[]> {
  const response = await apiClient.get(`/api/customers/${customerId}/notes`)
  return response.data
}

export async function addCustomerNote(customerId: string, content: string): Promise<CustomerNote> {
  const response = await apiClient.post(`/api/customers/${customerId}/notes`, { content })
  return response.data
}
