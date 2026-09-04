import { Navigate, Outlet } from 'react-router-dom'

import { useAuth } from '@/features/auth/hooks/useAuth'
import { LoadingState } from '@/components/ui/LoadingState'

/**
 * Layout-route guard for unauthenticated-only routes (e.g. /login). Sends an
 * already-authenticated user back to the app instead of showing them the
 * sign-in screen again.
 */
export function PublicRoute() {
  const { status } = useAuth()

  // While AuthProvider is still trying to silently restore a session from
  // the refresh-token cookie (see AuthProvider.tsx), avoid flashing the
  // login form at an already-signed-in user who reloaded on /login.
  if (status === 'idle') {
    return <LoadingState />
  }

  if (status === 'authenticated') {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
