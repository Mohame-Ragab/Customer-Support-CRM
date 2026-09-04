import { apiClient } from '@/lib/api/apiClient'

export type ChangePasswordPayload = {
  currentPassword: string
  newPassword: string
}

export async function changePassword(payload: ChangePasswordPayload): Promise<void> {
  await apiClient.post('/api/v1/auth/change-password', payload)
}
