import { apiClient } from '@/lib/api/apiClient'
import type { CreateUserInput, ListUsersParams, PagedResult, UpdateUserInput, User } from '../types'

export async function listUsers(params: ListUsersParams): Promise<PagedResult<User>> {
  const response = await apiClient.get('/api/users', { params })
  return response.data
}

export async function getUser(id: string): Promise<User> {
  const response = await apiClient.get(`/api/users/${id}`)
  return response.data
}

export async function createUser(input: CreateUserInput): Promise<User> {
  const response = await apiClient.post('/api/users', input)
  return response.data
}

export async function updateUser(id: string, input: UpdateUserInput): Promise<User> {
  const response = await apiClient.put(`/api/users/${id}`, input)
  return response.data
}
