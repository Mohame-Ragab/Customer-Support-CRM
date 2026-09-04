import { apiClient } from '@/lib/api/apiClient'

/**
 * Fire-and-forget server-side logout. The client-side session ends regardless
 * of the network result (see AuthProvider.logout). Errors are swallowed so a
 * dropped connection cannot leave the user "stuck signed in".
 */
export async function logoutRequest(): Promise<void> {
  try {
    await apiClient.post('/api/v1/auth/logout')
  } catch {
    // Intentional: logout must always succeed on the client.
  }
}
