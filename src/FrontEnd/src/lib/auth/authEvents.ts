/**
 * Minimal pub/sub used to let the Axios layer (lib/api/apiClient.ts) signal
 * "the session is no longer valid" without importing React or React Router.
 * The Axios client stays a plain HTTP module; AuthProvider (features/auth) is
 * the only subscriber, and it decides what a lost session means for the UI
 * (clearing auth state, which ProtectedRoute then reacts to by redirecting).
 * This keeps routing decisions out of the low-level HTTP layer.
 */

type Listener = () => void

const listeners = new Set<Listener>()

/** Called by apiClient's response interceptor on a 401. */
export function notifyUnauthorized(): void {
  for (const listener of listeners) listener()
}

/** Called by AuthProvider to react to a lost session. Returns an unsubscribe function. */
export function onUnauthorized(listener: Listener): () => void {
  listeners.add(listener)
  return () => listeners.delete(listener)
}
