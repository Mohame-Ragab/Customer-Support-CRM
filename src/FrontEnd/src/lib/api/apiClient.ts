import axios from 'axios'

import { appConfig } from '@/config/appConfig'
import { env } from '@/config/env'
import i18n from '@/lib/i18n/i18n'
import { notifyUnauthorized } from '@/lib/auth/authEvents'
import { getAccessToken, setAccessToken, clearAccessToken } from '@/lib/auth/tokenStorage'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { refreshSession } from '@/features/auth/api/refreshSession'

/**
 * The single Axios instance every API call in the app must go through.
 * Contains only generic HTTP plumbing - no CRM business logic, and no
 * feature-specific request functions (customersApi, ticketsApi, ...) belong
 * here; those are added per feature once that feature is implemented.
 */
export const apiClient = axios.create({
  baseURL: env.apiBaseUrl,
  timeout: appConfig.apiTimeoutMs,
  headers: {
    'Content-Type': 'application/json',
  },
  // The refresh token now travels as an HttpOnly cookie (see
  // lib/auth/tokenStorage.ts) - the browser only attaches it automatically
  // when the request opts in to sending credentials. The backend's CORS
  // policy already allows credentials for its configured origins.
  withCredentials: true,
})

// Attach the bearer token (if any) and the current UI language, so the
// backend's Accept-Language-driven localization (see src/BackEnd/docs,
// "Localization") matches whatever the user has selected on the frontend.
apiClient.interceptors.request.use((config) => {
  const token = getAccessToken()
  if (token) {
    config.headers.set('Authorization', `Bearer ${token}`)
  }
  config.headers.set('Accept-Language', i18n.language)
  return config
})

// In-flight refresh promise to prevent concurrent refresh requests.
let refreshPromise: Promise<boolean> | null = null

// Normalize every failure to ApiError and react to a lost session. Deliberately
// does NOT redirect or touch React Router here - see lib/auth/authEvents.ts.
apiClient.interceptors.response.use(
  (response) => response,
  async (error: unknown) => {
    const apiError = normalizeApiError(error)

    if (apiError.status === 401) {
      // Prevent multiple simultaneous refresh requests.
      if (refreshPromise) {
        try {
          const success = await refreshPromise
          if (success) {
            // Refresh succeeded; retry the original request.
            const errorWithConfig = error as any
            const config = errorWithConfig.config
            if (config) {
              return apiClient(config)
            }
          }
        } catch {
          // Refresh failed; fall through to unauthorized.
        }
      } else {
        // Attempt to refresh the session. The refresh token itself is an
        // HttpOnly cookie (see lib/auth/tokenStorage.ts) - never readable or
        // checkable from JS, so this always calls the endpoint (relying on
        // the browser attaching the cookie via withCredentials) rather than
        // pre-checking for a stored token the way this used to.
        const errorConfig = (error as any)?.config
        if (errorConfig?._retry) {
          // Already retried once for this request; do not loop.
          notifyUnauthorized()
        } else {
          refreshPromise = (async () => {
            try {
              const refreshResult = await refreshSession()
              setAccessToken(refreshResult.accessToken)
              return true
            } catch (refreshError) {
              // Refresh failed (no/expired/invalid cookie); clear the
              // in-memory access token and notify.
              clearAccessToken()
              notifyUnauthorized()
              throw refreshError
            }
          })()

          try {
            const success = await refreshPromise
            if (success && errorConfig) {
              errorConfig.headers.set('Authorization', `Bearer ${getAccessToken()}`)
              errorConfig._retry = true
              return await apiClient(errorConfig)
            }
          } catch {
            // Fall through to reject with apiError.
          } finally {
            refreshPromise = null
          }
        }
      }
    }

    return Promise.reject(apiError)
  },
)
