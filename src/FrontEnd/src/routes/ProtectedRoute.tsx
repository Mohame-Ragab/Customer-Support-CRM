import { Navigate, Outlet, useLocation } from 'react-router-dom'

import { useAuth } from '@/features/auth/hooks/useAuth'
import { hasAnyRole } from '@/features/auth/authorization'
import { LoadingState } from '@/components/ui/LoadingState'

interface ProtectedRouteProps {
  /**
   * Optional role allow-list (e.g. `[Roles.Admin]`). When provided, an
   * authenticated user who lacks every listed role is redirected to "/"
   * instead of the wrapped route rendering. Omit for "any authenticated
   * user" routes - existing behavior is unchanged when this prop is absent.
   */
  allowedRoles?: string[]
}

/**
 * Layout-route guard for authenticated-only routes. Redirects to /login,
 * preserving the attempted location so a future login flow can return the
 * user to where they were headed. This is a UX convenience only - the
 * backend independently rejects unauthenticated requests; see
 * docs/frontend-architecture.md, "Security boundaries".
 */
export function ProtectedRoute({ allowedRoles }: ProtectedRouteProps = {}) {
  const { status, user } = useAuth()
  const location = useLocation()

  // Bug fix: previously redirected to /login for any status !== 'authenticated',
  // including 'idle' - the state AuthProvider starts in while it is still
  // trying to silently restore a session from the refresh-token cookie after
  // a page reload. That redirect fired before the restoration attempt could
  // ever complete, so a valid session was always lost on reload. Now waits
  // for the attempt to resolve to 'authenticated' or 'unauthenticated'.
  if (status === 'idle') {
    return <LoadingState />
  }

  if (status !== 'authenticated') {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  if (allowedRoles && allowedRoles.length > 0 && !hasAnyRole(user, allowedRoles)) {
    return <Navigate to="/" replace />
  }

  return <Outlet />
}
