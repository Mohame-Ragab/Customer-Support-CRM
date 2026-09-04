import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react'

import { logoutRequest } from '@/features/auth/api/logout'
import { refreshSession } from '@/features/auth/api/refreshSession'
import { onUnauthorized } from '@/lib/auth/authEvents'
import { decodeJwtPayload, isTokenExpired, rolesFromClaims } from '@/lib/auth/jwt'
import { getAccessToken, setAccessToken, clearAccessToken } from '@/lib/auth/tokenStorage'
import type { AuthState, AuthUser } from '@/types/auth'

import { AuthContext, type AuthContextValue } from './AuthContext'

function userFromToken(token: string): AuthUser | null {
  const claims = decodeJwtPayload(token)
  if (!claims?.sub || isTokenExpired(claims)) return null

  return {
    id: claims.sub,
    userName: claims.name,
    email: claims.email,
    roles: rolesFromClaims(claims),
  }
}

interface AuthProviderProps {
  children: ReactNode
}

/**
 * Owns authentication *state* for the whole app - see src/BackEnd's JWT setup
 * and docs/frontend-architecture.md, "Authentication architecture".
 * Subscribes to lib/auth/authEvents so a 401 from any API call (see
 * lib/api/apiClient.ts) clears the session automatically.
 *
 * Bug fix (session lost on page reload): the access token lives in memory
 * only and does not survive a reload, by design (see tokenStorage.ts) - but
 * this provider's initial state used to synchronously resolve to
 * 'unauthenticated' whenever no in-memory token was present (always true
 * right after a reload), and ProtectedRoute redirects to /login the instant
 * status !== 'authenticated'. That redirect fired before the "bootstrap
 * silent refresh" effect below even had a chance to run - and even then, the
 * old bootstrap effect only attempted a refresh when a refresh token was
 * already in memory, which (after a reload) it never was either, since the
 * refresh token used to live in memory too. Fixed on both sides: the refresh
 * token is now an HttpOnly cookie (survives reload; see refreshSession.ts),
 * and this provider starts in the pre-existing-but-previously-unused 'idle'
 * status (see types/auth.ts) whenever it cannot already resolve a user
 * synchronously, so ProtectedRoute waits for the bootstrap attempt to finish
 * instead of redirecting first.
 */
export function AuthProvider({ children }: AuthProviderProps) {
  const [state, setState] = useState<AuthState>(() => {
    const existingToken = getAccessToken()
    const user = existingToken ? userFromToken(existingToken) : null
    return user ? { status: 'authenticated', user } : { status: 'idle', user: null }
  })

  // Bootstrap: on app start, if not already authenticated from an in-memory
  // token, attempt a silent refresh via the HttpOnly cookie. This is what
  // restores the session after a page reload.
  useEffect(() => {
    if (state.status !== 'idle') {
      return
    }

    let cancelled = false

    const bootstrap = async () => {
      try {
        const refreshResult = await refreshSession()
        if (cancelled) return

        setAccessToken(refreshResult.accessToken)
        const user = userFromToken(refreshResult.accessToken)
        setState(user ? { status: 'authenticated', user } : { status: 'unauthenticated', user: null })
      } catch {
        // No valid session to restore (no cookie, expired, or revoked) -
        // this is the normal case for a first-time/logged-out visitor, not
        // an error.
        if (!cancelled) {
          setState({ status: 'unauthenticated', user: null })
        }
      }
    }

    void bootstrap()

    return () => {
      cancelled = true
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [])

  const logout = useCallback(() => {
    // Fire-and-forget; do not await. See auth/logout intake — subsequent requests
    // must simply be unauthorized after natural expiry; server call is best-effort.
    void logoutRequest()
    clearAccessToken()
    setState({ status: 'unauthenticated', user: null })
  }, [])

  const login = useCallback(
    (accessToken: string) => {
      const user = userFromToken(accessToken)
      if (!user) {
        logout()
        return
      }
      setAccessToken(accessToken)
      setState({ status: 'authenticated', user })
    },
    [logout],
  )

  useEffect(() => onUnauthorized(logout), [logout])

  const value = useMemo<AuthContextValue>(
    () => ({ ...state, login, logout }),
    [state, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
