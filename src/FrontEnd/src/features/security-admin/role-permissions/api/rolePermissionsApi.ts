import { apiClient } from '@/lib/api/apiClient'
import type { RolePermissionsDto } from '../types'

export async function getPermissionCatalog(): Promise<string[]> {
  const response = await apiClient.get('/api/admin/role-permissions/catalog')
  return response.data
}

export async function getRolePermissions(roleId: string): Promise<RolePermissionsDto> {
  const response = await apiClient.get(`/api/admin/role-permissions/${roleId}`)
  return response.data
}

export async function assignPermission(roleId: string, permissionName: string): Promise<void> {
  await apiClient.post(`/api/admin/role-permissions/${roleId}`, { permissionName })
}

export async function removePermission(roleId: string, permissionName: string): Promise<void> {
  await apiClient.delete(`/api/admin/role-permissions/${roleId}/${encodeURIComponent(permissionName)}`)
}
