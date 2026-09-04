import { useEffect } from 'react'
import { useForm, Controller } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  MenuItem,
  Stack,
  FormControlLabel,
  Switch,
  Alert,
  CircularProgress,
} from '@mui/material'
import { USER_ROLES, type User, type UserRole } from '../types'

interface UserFormDialogProps {
  open: boolean
  onClose: () => void
  onSubmit: (data: { fullName: string; email: string; role: UserRole; initialPassword: string; isActive: boolean }) => void
  user?: User | null
  isPending?: boolean
  errorMessage?: string | null
}

interface FormValues {
  fullName: string
  email: string
  role: UserRole
  initialPassword: string
  isActive: boolean
}

export function UserFormDialog({ open, onClose, onSubmit, user, isPending, errorMessage }: UserFormDialogProps) {
  const { t } = useTranslation()
  const isEdit = !!user

  const { register, handleSubmit, control, reset, formState: { errors } } = useForm<FormValues>({
    defaultValues: {
      fullName: '',
      email: '',
      role: 'Agent',
      initialPassword: '',
      isActive: true,
    },
  })

  useEffect(() => {
    if (open) {
      reset({
        fullName: user?.fullName ?? '',
        email: user?.email ?? '',
        role: (user?.role as UserRole) ?? 'Agent',
        initialPassword: '',
        isActive: user?.isActive ?? true,
      })
    }
  }, [open, user, reset])

  const submit = (data: FormValues) => onSubmit(data)

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>{isEdit ? t('users.edit') : t('users.new')}</DialogTitle>
      <form onSubmit={handleSubmit(submit)}>
        <DialogContent>
          <Stack spacing={2} sx={{ mt: 1 }}>
            {errorMessage && <Alert severity="error">{errorMessage}</Alert>}

            <TextField
              {...register('fullName', { required: true, minLength: 2, maxLength: 100 })}
              label={t('users.form.fullName')}
              fullWidth
              disabled={isPending}
              error={!!errors.fullName}
              helperText={errors.fullName ? t('users.errors.fullNameRequired') : undefined}
            />

            <TextField
              {...register('email', { required: !isEdit })}
              label={t('users.form.email')}
              type="email"
              fullWidth
              disabled={isPending || isEdit}
              error={!!errors.email}
              helperText={errors.email ? t('users.errors.emailInvalid') : undefined}
            />

            <Controller
              name="role"
              control={control}
              rules={{ required: true }}
              render={({ field }) => (
                <TextField
                  {...field}
                  select
                  label={t('users.form.role')}
                  fullWidth
                  disabled={isPending}
                  error={!!errors.role}
                  helperText={errors.role ? t('users.errors.roleInvalid') : undefined}
                >
                  {USER_ROLES.map((role) => (
                    <MenuItem key={role} value={role}>
                      {t(`users.roles.${role.toLowerCase()}`)}
                    </MenuItem>
                  ))}
                </TextField>
              )}
            />

            {!isEdit && (
              <TextField
                {...register('initialPassword', { required: !isEdit, minLength: 8 })}
                label={t('users.form.initialPassword')}
                type="password"
                fullWidth
                disabled={isPending}
                error={!!errors.initialPassword}
                helperText={errors.initialPassword ? t('users.errors.passwordWeak') : undefined}
              />
            )}

            {isEdit && (
              <Controller
                name="isActive"
                control={control}
                render={({ field }) => (
                  <FormControlLabel
                    control={<Switch checked={field.value} onChange={(e) => field.onChange(e.target.checked)} disabled={isPending} />}
                    label={t('users.form.isActive')}
                  />
                )}
              />
            )}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={onClose} disabled={isPending}>
            {t('users.actions.cancel', { defaultValue: 'Cancel' })}
          </Button>
          <Button type="submit" variant="contained" disabled={isPending}>
            {isPending ? <CircularProgress size={20} /> : t('users.actions.save', { defaultValue: 'Save' })}
          </Button>
        </DialogActions>
      </form>
    </Dialog>
  )
}
