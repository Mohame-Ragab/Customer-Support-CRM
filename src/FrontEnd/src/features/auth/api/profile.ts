import { apiClient } from '@/lib/api/apiClient'

export interface UserProfile {
  id: string
  userName: string | null
  email: string | null
  emailConfirmed: boolean
  phoneNumber: string | null
  roles: string[]
}

export interface UpdateProfileRequest {
  email?: string | null
  phoneNumber?: string | null
}

export async function getProfile(): Promise<UserProfile> {
  const response = await apiClient.get('/api/users/me')
  return response.data
}

export async function updateProfile(data: UpdateProfileRequest): Promise<UserProfile> {
  const response = await apiClient.put('/api/users/me', data)
  return response.data
}
