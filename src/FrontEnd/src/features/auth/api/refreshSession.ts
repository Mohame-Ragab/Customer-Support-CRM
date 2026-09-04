import axios from 'axios'
import { env } from '@/config/env'
import { appConfig } from '@/config/appConfig'

export interface RefreshResponse {
  accessToken: string
  accessTokenExpiresAt: string
  refreshToken: string
  refreshTokenExpiresAt: string
}

/**
 * Bare axios instance for refresh requests - NOT the main apiClient.
 * This avoids triggering the response interceptor during a refresh attempt,
 * which could cause infinite 401→refresh→401 loops.
 */
const refreshClient = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: appConfig.apiTimeoutMs,
  headers: {
    'Content-Type': 'application/json',
  },
  // The refresh token travels as an HttpOnly cookie the backend set on
  // /login or a previous /refresh (see AuthController.cs) - this is what
  // lets the browser attach it automatically; there is no token to pass in
  // the request body anymore (see lib/auth/tokenStorage.ts).
  withCredentials: true,
})

/**
 * Attempt to refresh the session using the HttpOnly refresh-token cookie.
 * Called by the 401 interceptor in apiClient, and by AuthProvider on app
 * boot to silently restore a session after a page reload.
 * Throws if refresh fails (no/expired/invalid cookie, network error, etc).
 */
export async function refreshSession(): Promise<RefreshResponse> {
  const { data } = await refreshClient.post<RefreshResponse>('/api/v1/auth/refresh', {})
  return data
}
