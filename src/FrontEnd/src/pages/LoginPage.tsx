import { useForm } from 'react-hook-form'
import { useTranslation } from 'react-i18next'
import { Link as RouterLink } from 'react-router-dom'
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
  Link,
} from '@mui/material'
import { useLoginMutation } from '@/features/auth/hooks/useLoginMutation'

interface LoginFormData {
  email: string
  password: string
}

export function LoginPage() {
  const { t } = useTranslation()
  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<LoginFormData>({
    defaultValues: { email: '', password: '' },
  })

  const { mutate, isPending, error } = useLoginMutation()

  const onSubmit = (data: LoginFormData) => {
    mutate(data)
  }

  const getErrorMessage = (): string => {
    if (!error) return ''

    // Map backend error codes to i18n keys
    const errorMap: Record<string, string> = {
      Auth_InvalidCredentials: t('auth.login.errors.invalidCredentials'),
      Auth_EmailNotVerified: t('auth.login.errors.emailNotVerified'),
      Auth_AccountLocked: t('auth.login.errors.accountLocked'),
    }

    return errorMap[error.code] || error.message || t('auth.login.errors.unknown')
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
              {t('auth.login.title')}
            </Typography>

            <form onSubmit={handleSubmit(onSubmit)}>
              <Stack spacing={2}>
                {error && <Alert severity="error">{getErrorMessage()}</Alert>}

                <TextField
                  {...register('email', {
                    required: true,
                    pattern: {
                      value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                      message: t('auth.login.errors.invalidCredentials'),
                    },
                  })}
                  label={t('auth.login.email')}
                  type="email"
                  fullWidth
                  disabled={isPending}
                  error={!!errors.email}
                  helperText={errors.email?.message}
                />

                <TextField
                  {...register('password', { required: true })}
                  label={t('auth.login.password')}
                  type="password"
                  fullWidth
                  disabled={isPending}
                  error={!!errors.password}
                  helperText={errors.password?.message}
                />

                <Button
                  variant="text"
                  component={RouterLink}
                  to="/forgot-password"
                  size="small"
                  sx={{ alignSelf: 'flex-end' }}
                >
                  {t('auth.login.forgotPassword')}
                </Button>

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
                      <span style={{ visibility: 'hidden' }}>{t('auth.login.submitting')}</span>
                    </>
                  ) : (
                    t('auth.login.submit')
                  )}
                </Button>
              </Stack>
            </form>

            <Typography align="center">
              {t('auth.login.noAccount')}{' '}
              <Link component={RouterLink} to="/register" underline="hover">
                {t('auth.login.registerLink')}
              </Link>
            </Typography>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}
