import { useState } from 'react'
import { useQuery } from '@tanstack/react-query'
import { useTranslation } from 'react-i18next'
import { Container, Typography, TextField, MenuItem, Box } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { listRoles } from '@/features/security-admin/role-permissions/api/rolesApi'
import { RolePermissionsEditor } from '@/features/security-admin/role-permissions/RolePermissionsEditor'

export function RolePermissionsPage() {
  const { t } = useTranslation()
  const [selectedRoleId, setSelectedRoleId] = useState('')

  const {
    data: roles,
    isLoading,
    isError,
    error,
  } = useQuery({
    queryKey: ['roles'],
    queryFn: listRoles,
  })

  const selectedRole = roles?.find((r) => r.id === selectedRoleId)

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('rolePermissions.pageTitle', { defaultValue: 'Role Permissions' })}
      </Typography>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState title={t('errors.unexpected')} message={normalizeApiError(error).detail} />
      )}

      {!isLoading && !isError && roles && roles.length === 0 && (
        <EmptyState title={t('rolePermissions.noRoles', { defaultValue: 'No roles found.' })} />
      )}

      {!isLoading && !isError && roles && roles.length > 0 && (
        <>
          <TextField
            select
            label={t('rolePermissions.selectRole', { defaultValue: 'Select a role' })}
            value={selectedRoleId}
            onChange={(e) => setSelectedRoleId(e.target.value)}
            sx={{ minWidth: 240, mb: 3 }}
          >
            {roles.map((role) => (
              <MenuItem key={role.id} value={role.id}>
                {role.name}
              </MenuItem>
            ))}
          </TextField>

          <Box>
            {selectedRole && (
              <RolePermissionsEditor roleId={selectedRole.id} roleName={selectedRole.name} />
            )}
          </Box>
        </>
      )}
    </Container>
  )
}
