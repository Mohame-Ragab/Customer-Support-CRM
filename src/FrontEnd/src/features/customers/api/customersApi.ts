import { apiClient } from '@/lib/api/apiClient'
import type {
  Customer,
  CreateCustomerInput,
  UpdateCustomerInput,
  PagedResult,
} from '../types/customer'

export async function getCustomerById(id: string): Promise<Customer> {
  const response = await apiClient.get(`/api/customers/${id}`)
  return response.data
}

export async function listCustomers(params: {
  page: number
  pageSize: number
}): Promise<PagedResult<Customer>> {
  const response = await apiClient.get('/api/customers', { params })
  return response.data
}

export async function createCustomer(input: CreateCustomerInput): Promise<Customer> {
  const response = await apiClient.post('/api/customers', input)
  return response.data
}

export async function updateCustomer(id: string, input: UpdateCustomerInput): Promise<Customer> {
  const response = await apiClient.put(`/api/customers/${id}`, input)
  return response.data
}
