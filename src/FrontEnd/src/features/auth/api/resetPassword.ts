import { apiClient } from '@/lib/api/apiClient'

export interface ResetPasswordRequest {
  email: string
  token: string
  newPassword: string
}

export async function postResetPassword(body: ResetPasswordRequest): Promise<void> {
  await apiClient.post('/api/v1/auth/reset-password', body)
}
