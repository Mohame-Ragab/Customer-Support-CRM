import { apiClient } from '@/lib/api/apiClient'

export interface VerifyEmailRequest {
  userId: string
  token: string
}

export interface ResendVerificationRequest {
  email: string
}

export async function postVerifyEmail(body: VerifyEmailRequest): Promise<void> {
  await apiClient.post('/api/v1/auth/verify-email', body)
}

export async function postResendVerification(body: ResendVerificationRequest): Promise<void> {
  await apiClient.post('/api/v1/auth/resend-verification', body)
}
