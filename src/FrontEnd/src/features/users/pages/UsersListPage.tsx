import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Container,
  Typography,
  Box,
  Button,
  TextField,
  MenuItem,
  Stack,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TablePagination,
  Chip,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useUsersList, useCreateUser, useUpdateUser } from '../hooks/useUsers'
import { UserFormDialog } from '../components/UserFormDialog'
import { USER_ROLES, type User, type UserRole } from '../types'

export function UsersListPage() {
  const { t } = useTranslation()
  const [page, setPage] = useState(0)
  const [pageSize, setPageSize] = useState(20)
  const [search, setSearch] = useState('')
  const [roleFilter, setRoleFilter] = useState('')
  const [dialogOpen, setDialogOpen] = useState(false)
  const [editingUser, setEditingUser] = useState<User | null>(null)
  const [formError, setFormError] = useState<string | null>(null)

  const { data, isLoading, isError, error, refetch } = useUsersList({
    page: page + 1,
    pageSize,
    search: search || undefined,
    role: roleFilter || undefined,
  })

  const createUser = useCreateUser()
  const updateUser = useUpdateUser(editingUser?.id ?? '')

  const openCreateDialog = () => {
    setEditingUser(null)
    setFormError(null)
    setDialogOpen(true)
  }

  const openEditDialog = (user: User) => {
    setEditingUser(user)
    setFormError(null)
    setDialogOpen(true)
  }

  const handleSubmit = (formData: {
    fullName: string
    email: string
    role: UserRole
    initialPassword: string
    isActive: boolean
  }) => {
    setFormError(null)
    if (editingUser) {
      updateUser.mutate(
        { fullName: formData.fullName, role: formData.role, isActive: formData.isActive },
        {
          onSuccess: () => setDialogOpen(false),
          onError: (err) => setFormError(normalizeApiError(err).detail ?? t('users.errors.updateFailed', { defaultValue: 'Failed to update user.' })),
        },
      )
    } else {
      createUser.mutate(
        { fullName: formData.fullName, email: formData.email, role: formData.role, initialPassword: formData.initialPassword },
        {
          onSuccess: () => setDialogOpen(false),
          onError: (err) => {
            const apiError = normalizeApiError(err)
            if (apiError.status === 409) {
              setFormError(t('users.errors.emailInUse'))
            } else {
              setFormError(apiError.detail ?? t('users.errors.createFailed', { defaultValue: 'Failed to create user.' }))
            }
          },
        },
      )
    }
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          {t('users.title')}
        </Typography>
        <Button variant="contained" onClick={openCreateDialog}>
          {t('users.new')}
        </Button>
      </Stack>

      <Stack direction="row" spacing={2} sx={{ mb: 3 }}>
        <TextField
          label={t('users.search', { defaultValue: 'Search' })}
          value={search}
          onChange={(e) => {
            setSearch(e.target.value)
            setPage(0)
          }}
          size="small"
        />
        <TextField
          select
          label={t('users.columns.role')}
          value={roleFilter}
          onChange={(e) => {
            setRoleFilter(e.target.value)
            setPage(0)
          }}
          size="small"
          sx={{ minWidth: 160 }}
        >
          <MenuItem value="">{t('users.allRoles', { defaultValue: 'All roles' })}</MenuItem>
          {USER_ROLES.map((role) => (
            <MenuItem key={role} value={role}>
              {t(`users.roles.${role.toLowerCase()}`)}
            </MenuItem>
          ))}
        </TextField>
      </Stack>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.items.length === 0 && (
        <EmptyState title={t('users.empty', { defaultValue: 'No users found.' })} />
      )}

      {!isLoading && !isError && data && data.items.length > 0 && (
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('users.columns.name')}</TableCell>
                <TableCell>{t('users.columns.email')}</TableCell>
                <TableCell>{t('users.columns.role')}</TableCell>
                <TableCell>{t('users.columns.active')}</TableCell>
                <TableCell>{t('users.columns.created')}</TableCell>
                <TableCell align="right">{t('users.actions.edit', { defaultValue: 'Edit' })}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.items.map((user) => (
                <TableRow key={user.id} hover>
                  <TableCell>{user.fullName ?? '—'}</TableCell>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{t(`users.roles.${user.role.toLowerCase()}`, { defaultValue: user.role })}</TableCell>
                  <TableCell>
                    <Chip
                      size="small"
                      label={user.isActive ? t('users.status.active', { defaultValue: 'Active' }) : t('users.status.inactive', { defaultValue: 'Inactive' })}
                      color={user.isActive ? 'success' : 'default'}
                      variant="outlined"
                    />
                  </TableCell>
                  <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
                  <TableCell align="right">
                    <Button size="small" onClick={() => openEditDialog(user)}>
                      {t('users.actions.edit', { defaultValue: 'Edit' })}
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
          <TablePagination
            component="div"
            count={data.totalCount}
            page={page}
            onPageChange={(_, newPage) => setPage(newPage)}
            rowsPerPage={pageSize}
            onRowsPerPageChange={(e) => {
              setPageSize(parseInt(e.target.value, 10))
              setPage(0)
            }}
          />
        </Box>
      )}

      <UserFormDialog
        open={dialogOpen}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
        user={editingUser}
        isPending={createUser.isPending || updateUser.isPending}
        errorMessage={formError}
      />
    </Container>
  )
}
