import { useState } from 'react'
import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { Link } from 'react-router-dom'
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
} from '@mui/material'
import { postForgotPassword } from '@/features/auth/api/forgotPassword'

interface ForgotPasswordFormData {
  email: string
}

export function ForgotPasswordPage() {
  const { t } = useTranslation()
  const [submitted, setSubmitted] = useState(false)
  const [error, setError] = useState('')

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<ForgotPasswordFormData>({
    defaultValues: { email: '' },
  })

  const [isPending, setIsPending] = useState(false)

  const onSubmit = async (data: ForgotPasswordFormData) => {
    setIsPending(true)
    setError('')
    try {
      await postForgotPassword({ email: data.email })
      setSubmitted(true)
      reset()
    } catch (err: any) {
      // Generic error handling - don't reveal if email exists or not
      setError(t('auth.forgotPassword.errors.generic'))
    } finally {
      setIsPending(false)
    }
  }

  if (submitted) {
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
                <AlertTitle>{t('auth.forgotPassword.successTitle')}</AlertTitle>
                {t('auth.forgotPassword.successMessage')}
              </Alert>

              <Typography variant="body2" color="textSecondary" align="center">
                {t('auth.forgotPassword.checkEmail')}
              </Typography>

              <Button variant="contained" fullWidth component={Link} to="/login">
                {t('auth.forgotPassword.backToLogin')}
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
              {t('auth.forgotPassword.title')}
            </Typography>

            <Typography variant="body2" color="textSecondary" align="center">
              {t('auth.forgotPassword.instructions')}
            </Typography>

            <form onSubmit={handleSubmit(onSubmit)}>
              <Stack spacing={2}>
                {error && <Alert severity="error">{error}</Alert>}

                <TextField
                  {...register('email', {
                    required: true,
                    pattern: {
                      value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                      message: t('auth.forgotPassword.errors.invalidEmail'),
                    },
                  })}
                  label={t('auth.forgotPassword.email')}
                  type="email"
                  fullWidth
                  disabled={isPending}
                  error={!!errors.email}
                  helperText={errors.email?.message}
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
                        {t('auth.forgotPassword.submitting')}
                      </span>
                    </>
                  ) : (
                    t('auth.forgotPassword.submit')
                  )}
                </Button>
              </Stack>
            </form>

            <Button variant="text" fullWidth component={Link} to="/login" size="small">
              {t('auth.forgotPassword.backToLogin')}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}
