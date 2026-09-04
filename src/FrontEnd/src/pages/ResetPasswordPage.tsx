import { useState } from 'react'
import { useSearchParams, Link } from 'react-router-dom'
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
  Card,
  CardContent,
  AlertTitle,
  IconButton,
  InputAdornment,
} from '@mui/material'
import { Visibility, VisibilityOff } from '@mui/icons-material'
import { postResetPassword } from '@/features/auth/api/resetPassword'

interface ResetPasswordFormData {
  newPassword: string
  confirmPassword: string
}

export function ResetPasswordPage() {
  const { t } = useTranslation()
  const [searchParams] = useSearchParams()
  const [state, setState] = useState<'form' | 'success' | 'error'>('form')
  const [errorMessage, setErrorMessage] = useState('')
  const [showPassword, setShowPassword] = useState(false)
  const [showConfirmPassword, setShowConfirmPassword] = useState(false)

  const {
    register,
    handleSubmit,
    formState: { errors },
    watch,
    reset,
  } = useForm<ResetPasswordFormData>({
    defaultValues: { newPassword: '', confirmPassword: '' },
  })

  const email = searchParams.get('email')
  const token = searchParams.get('token')
  const newPassword = watch('newPassword')
  const [isPending, setIsPending] = useState(false)

  if (!email || !token) {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '100vh',
          p: 2,
        }}
      >
        <Card sx={{ width: '100%', maxWidth: 400 }}>
          <CardContent>
            <Stack spacing={3}>
              <Alert severity="error">
                <AlertTitle>{t('auth.resetPassword.errorTitle')}</AlertTitle>
                {t('auth.resetPassword.invalidLink')}
              </Alert>
              <Button variant="contained" fullWidth component={Link} to="/login">
                {t('auth.resetPassword.backToLogin')}
              </Button>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    )
  }

  const onSubmit = async (data: ResetPasswordFormData) => {
    setIsPending(true)
    setErrorMessage('')
    try {
      await postResetPassword({
        email,
        token,
        newPassword: data.newPassword,
      })
      setState('success')
      reset()
    } catch (err: any) {
      setState('error')
      setErrorMessage(
        err?.response?.data?.message || err?.message || t('auth.resetPassword.errors.generic'),
      )
    } finally {
      setIsPending(false)
    }
  }

  if (state === 'success') {
    return (
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'center',
          alignItems: 'center',
          minHeight: '100vh',
          p: 2,
        }}
      >
        <Card sx={{ width: '100%', maxWidth: 400 }}>
          <CardContent>
            <Stack spacing={3}>
              <Alert severity="success">
                <AlertTitle>{t('auth.resetPassword.successTitle')}</AlertTitle>
                {t('auth.resetPassword.successMessage')}
              </Alert>

              <Button variant="contained" fullWidth component={Link} to="/login">
                {t('auth.resetPassword.backToLogin')}
              </Button>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    )
  }

  return (
    <Box
      sx={{
        display: 'flex',
        justifyContent: 'center',
        alignItems: 'center',
        minHeight: '100vh',
        p: 2,
      }}
    >
      <Card sx={{ width: '100%', maxWidth: 400 }}>
        <CardContent>
          <Stack spacing={3}>
            <Typography variant="h5" component="h1" align="center" gutterBottom>
              {t('auth.resetPassword.title')}
            </Typography>

            {state === 'error' && <Alert severity="error">{errorMessage}</Alert>}

            <form onSubmit={handleSubmit(onSubmit)}>
              <Stack spacing={2}>
                <TextField
                  {...register('newPassword', {
                    required: true,
                    minLength: {
                      value: 8,
                      message: t('auth.resetPassword.errors.passwordTooShort'),
                    },
                  })}
                  label={t('auth.resetPassword.newPassword')}
                  type={showPassword ? 'text' : 'password'}
                  fullWidth
                  disabled={isPending}
                  error={!!errors.newPassword}
                  helperText={errors.newPassword?.message}
                  slotProps={{
                    input: {
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton
                            edge="end"
                            onClick={() => setShowPassword(!showPassword)}
                            onMouseDown={(e) => e.preventDefault()}
                            disabled={isPending}
                            tabIndex={-1}
                          >
                            {showPassword ? <VisibilityOff /> : <Visibility />}
                          </IconButton>
                        </InputAdornment>
                      ),
                    },
                  }}
                />

                <TextField
                  {...register('confirmPassword', {
                    required: true,
                    validate: (value) =>
                      value === newPassword || t('auth.resetPassword.errors.passwordMismatch'),
                  })}
                  label={t('auth.resetPassword.confirmPassword')}
                  type={showConfirmPassword ? 'text' : 'password'}
                  fullWidth
                  disabled={isPending}
                  error={!!errors.confirmPassword}
                  helperText={errors.confirmPassword?.message}
                  slotProps={{
                    input: {
                      endAdornment: (
                        <InputAdornment position="end">
                          <IconButton
                            edge="end"
                            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                            onMouseDown={(e) => e.preventDefault()}
                            disabled={isPending}
                            tabIndex={-1}
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
                  disabled={isPending}
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
                        {t('auth.resetPassword.submitting')}
                      </span>
                    </>
                  ) : (
                    t('auth.resetPassword.submit')
                  )}
                </Button>
              </Stack>
            </form>

            <Button variant="text" fullWidth component={Link} to="/login" size="small">
              {t('auth.resetPassword.backToLogin')}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}
