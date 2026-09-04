import { apiClient } from '@/lib/api/apiClient'

export interface Role {
  id: string
  name: string
}

/**
 * Minimal role listing, scoped to this feature's role picker. Backed by
 * GET /api/roles (security-admin/manage-roles), which has no frontend of its
 * own yet - see that story's "No frontend changes required" note.
 */
export async function listRoles(): Promise<Role[]> {
  const response = await apiClient.get('/api/roles')
  return response.data
}
