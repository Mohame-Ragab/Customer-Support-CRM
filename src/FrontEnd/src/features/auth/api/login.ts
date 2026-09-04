import { apiClient } from '@/lib/api/apiClient'

export interface LoginRequestBody {
  email: string
  password: string
}

export interface LoginResponseBody {
  accessToken: string
  refreshToken: string
  expiresAtUtc: string
  tokenType: 'Bearer'
  userId: string
  email: string
  displayName: string
  roles: string[]
}

export async function postLogin(body: LoginRequestBody): Promise<LoginResponseBody> {
  const { data } = await apiClient.post<LoginResponseBody>('/api/v1/auth/login', body)
  return data
}
