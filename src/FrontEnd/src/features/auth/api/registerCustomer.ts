import { apiClient } from '@/lib/api/apiClient'
import type { RegisterCustomerRequest, RegisterCustomerResponse } from '@/types/auth'

export async function registerCustomer(payload: RegisterCustomerRequest): Promise<RegisterCustomerResponse> {
  const { data } = await apiClient.post<RegisterCustomerResponse>('/api/v1/auth/register', payload)
  return data
}
