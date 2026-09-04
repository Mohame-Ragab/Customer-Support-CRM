import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Box, Typography, FormGroup, FormControlLabel, Checkbox, Paper } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { usePermissionCatalog, useRolePermissions, useAssignPermission, useRemovePermission } from './hooks/useRolePermissions'

interface RolePermissionsEditorProps {
  roleId: string
  roleName: string
}

export function RolePermissionsEditor({ roleId, roleName }: RolePermissionsEditorProps) {
  const { t } = useTranslation()
  const [toggleError, setToggleError] = useState<string | null>(null)

  const { data: catalog, isLoading: catalogLoading, isError: catalogError, error: catalogErrorObj } = usePermissionCatalog()
  const { data: rolePermissions, isLoading: rpLoading, isError: rpError, error: rpErrorObj } = useRolePermissions(roleId)
  const assignPermission = useAssignPermission(roleId)
  const removePermission = useRemovePermission(roleId)

  const isLoading = catalogLoading || rpLoading
  const isError = catalogError || rpError

  const handleToggle = (permission: string, checked: boolean) => {
    setToggleError(null)
    const mutation = checked ? assignPermission : removePermission
    mutation.mutate(permission, {
      onError: (err) => {
        setToggleError(normalizeApiError(err).detail ?? t('rolePermissions.errors.toggleFailed', { defaultValue: 'Failed to update permission.' }))
      },
    })
  }

  if (isLoading) {
    return <LoadingState />
  }

  if (isError) {
    return (
      <ErrorState
        title={t('errors.unexpected')}
        message={normalizeApiError(catalogErrorObj ?? rpErrorObj).detail}
      />
    )
  }

  if (!catalog || catalog.length === 0) {
    return <EmptyState title={t('rolePermissions.empty', { defaultValue: 'No permissions defined.' })} />
  }

  const granted = new Set(rolePermissions?.permissions ?? [])

  const groups = catalog.reduce<Record<string, string[]>>((acc, permission) => {
    const group = permission.split('.')[0] ?? permission
    const list = acc[group] ?? []
    list.push(permission)
    acc[group] = list
    return acc
  }, {})

  return (
    <Box>
      <Typography variant="h6" gutterBottom>
        {t('rolePermissions.title', { defaultValue: 'Permissions' })} — {roleName}
      </Typography>

      {toggleError && (
        <ErrorState message={toggleError} />
      )}

      {Object.entries(groups).map(([group, permissions]) => (
        <Paper key={group} variant="outlined" sx={{ p: 2, mb: 2 }}>
          <Typography variant="subtitle2" color="text.secondary" gutterBottom sx={{ textTransform: 'capitalize' }}>
            {group}
          </Typography>
          <FormGroup>
            {permissions.map((permission) => (
              <FormControlLabel
                key={permission}
                control={
                  <Checkbox
                    checked={granted.has(permission)}
                    onChange={(e) => handleToggle(permission, e.target.checked)}
                    disabled={assignPermission.isPending || removePermission.isPending}
                  />
                }
                label={permission}
              />
            ))}
          </FormGroup>
        </Paper>
      ))}
    </Box>
  )
}
