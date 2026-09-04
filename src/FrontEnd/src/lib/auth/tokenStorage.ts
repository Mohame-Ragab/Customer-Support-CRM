/**
 * Centralized access-token storage. Nothing outside this module should read
 * or write the token directly (never call localStorage/sessionStorage for it
 * from a component or feature).
 *
 * Security design, now fully realized (previously documented as a future
 * target - see git history): the access token is kept in an in-memory module
 * variable only - never written to localStorage/sessionStorage, so it is
 * inaccessible to any other script on the page even under an XSS bug (unlike
 * localStorage, which is trivially readable by injected JS). The refresh
 * token is no longer handled here at all - the backend now sets it as an
 * HttpOnly, Secure (in production), SameSite=Lax cookie on /login and
 * /refresh (see AuthController.cs), scoped to the /api/v1/auth path. That
 * cookie is never readable from JavaScript and is sent automatically by the
 * browser (apiClient uses `withCredentials: true`); the frontend only ever
 * calls POST /api/v1/auth/refresh with no body and lets the cookie do the
 * rest. This is also what fixes session loss on page reload: the access
 * token does not survive a reload (by design), but the HttpOnly cookie does,
 * so AuthProvider's bootstrap effect can silently re-mint a fresh access
 * token from it - see AuthProvider.tsx.
 */

let accessToken: string | null = null

export function getAccessToken(): string | null {
  return accessToken
}

export function setAccessToken(token: string): void {
  accessToken = token
}

export function clearAccessToken(): void {
  accessToken = null
}
