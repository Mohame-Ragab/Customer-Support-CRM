import { apiClient } from '@/lib/api/apiClient'
import type { CustomerInteractionHistoryResult } from './types'

export async function getCustomerInteractionHistory(
  customerId: string,
  page = 1,
  pageSize = 20,
): Promise<CustomerInteractionHistoryResult> {
  const response = await apiClient.get(`/api/customers/${customerId}/interaction-history`, {
    params: { page, pageSize },
  })
  return response.data
}
