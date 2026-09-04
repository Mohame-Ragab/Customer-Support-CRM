import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query'
import { assignPermission, getPermissionCatalog, getRolePermissions, removePermission } from '../api/rolePermissionsApi'

export function usePermissionCatalog() {
  return useQuery({
    queryKey: ['permission-catalog'],
    queryFn: getPermissionCatalog,
  })
}

export function useRolePermissions(roleId: string | undefined) {
  return useQuery({
    queryKey: ['role-permissions', roleId],
    queryFn: () => getRolePermissions(roleId!),
    enabled: !!roleId,
  })
}

export function useAssignPermission(roleId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (permissionName: string) => assignPermission(roleId, permissionName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['role-permissions', roleId] })
    },
  })
}

export function useRemovePermission(roleId: string) {
  const queryClient = useQueryClient()
  return useMutation({
    mutationFn: (permissionName: string) => removePermission(roleId, permissionName),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['role-permissions', roleId] })
    },
  })
}
