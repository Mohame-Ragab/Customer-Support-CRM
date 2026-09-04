import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import {
  Box,
  Button,
  TextField,
  Alert,
  CircularProgress,
  Stack,
  Typography,
  Snackbar,
  IconButton,
  InputAdornment,
} from '@mui/material'
import { Visibility, VisibilityOff } from '@mui/icons-material'
import { useChangePassword } from '@/features/auth/hooks/useChangePassword'

interface ChangePasswordFormData {
  currentPassword: string
  newPassword: string
  confirmNewPassword: string
}

export function ChangePasswordForm() {
  const { t } = useTranslation()
  const { register, handleSubmit, watch, reset, formState: { errors } } = useForm<ChangePasswordFormData>({
    defaultValues: { currentPassword: '', newPassword: '', confirmNewPassword: '' },
  })

  const { mutate, isPending, error } = useChangePassword()
  const [showSuccess, setShowSuccess] = useState(false)
  const [showCurrentPassword, setShowCurrentPassword] = useState(false)
  const [showNewPassword, setShowNewPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  const newPassword = watch('newPassword')

  const onSubmit = (data: ChangePasswordFormData) => {
    if (data.newPassword !== data.confirmNewPassword) {
      return
    }

    mutate({
      currentPassword: data.currentPassword,
      newPassword: data.newPassword,
    })

    // Show success and reset form
    reset()
    setShowSuccess(true)
  }

  return (
    <Box sx={{ maxWidth: 400 }}>
      <Typography variant="h6" component="h2" gutterBottom>
        {t('auth.changePassword.title')}
      </Typography>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Stack spacing={2}>
          {error && (
            <Alert severity="error">
              {error.code === 'current_password_incorrect'
                ? t('auth.changePassword.errors.currentIncorrect')
                : t('auth.changePassword.errors.policy')}
            </Alert>
          )}

          <TextField
            {...register('currentPassword', { required: true })}
            label={t('auth.changePassword.fields.current')}
            type={showCurrentPassword ? 'text' : 'password'}
            fullWidth
            disabled={isPending}
            error={!!errors.currentPassword}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowCurrentPassword(!showCurrentPassword)}
                      edge="end"
                    >
                      {showCurrentPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
          />

          <TextField
            {...register('newPassword', {
              required: true,
              minLength: { value: 8, message: t('auth.changePassword.errors.policy') },
            })}
            label={t('auth.changePassword.fields.new')}
            type={showNewPassword ? 'text' : 'password'}
            fullWidth
            disabled={isPending}
            error={!!errors.newPassword}
            helperText={errors.newPassword?.message}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowNewPassword(!showNewPassword)}
                      edge="end"
                    >
                      {showNewPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
          />

          <TextField
            {...register('confirmNewPassword', {
              required: true,
              validate: (value) =>
                value === newPassword || t('auth.changePassword.errors.mismatch'),
            })}
            label={t('auth.changePassword.fields.confirm')}
            type={showConfirmPassword ? 'text' : 'password'}
            fullWidth
            disabled={isPending}
            error={!!errors.confirmNewPassword}
            helperText={errors.confirmNewPassword?.message}
            slotProps={{
              input: {
                endAdornment: (
                  <InputAdornment position="end">
                    <IconButton
                      onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                      edge="end"
                    >
                      {showConfirmPassword ? <VisibilityOff /> : <Visibility />}
                    </IconButton>
                  </InputAdornment>
                ),
              },
            }}
          />

          <Button
            type="submit"
            variant="contained"
            fullWidth
            disabled={isPending || !!errors.confirmNewPassword}
            sx={{ position: 'relative' }}
          >
            {isPending ? (
              <>
                <CircularProgress
                  size={24}
                  sx={{
                    position: 'absolute',
                    left: '50%',
                    marginLeft: '-12px',
                  }}
                />
                <span style={{ visibility: 'hidden' }}>
                  {t('auth.login.submitting')}
                </span>
              </>
            ) : (
              t('auth.changePassword.submit')
            )}
          </Button>
        </Stack>
      </form>

      <Snackbar
        open={showSuccess}
        autoHideDuration={6000}
        onClose={() => setShowSuccess(false)}
      >
        <Alert severity="success" onClose={() => setShowSuccess(false)}>
          {t('auth.changePassword.success')}
        </Alert>
      </Snackbar>
    </Box>
  )
}
