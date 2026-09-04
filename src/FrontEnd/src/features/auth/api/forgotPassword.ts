import { apiClient } from '@/lib/api/apiClient'

export interface ForgotPasswordRequest {
  email: string
}

export async function postForgotPassword(body: ForgotPasswordRequest): Promise<void> {
  await apiClient.post('/api/v1/auth/forgot-password', body)
}
