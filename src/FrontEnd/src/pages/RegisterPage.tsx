import { useForm } from 'react-hook-form'
import { useNavigate } from 'react-router-dom'
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
  Link,
} from '@mui/material'
import { useMutation } from '@tanstack/react-query'
import { registerCustomer } from '@/features/auth/api/registerCustomer'
import { AxiosError } from 'axios'

interface RegisterFormData {
  email: string
  password: string
  confirmPassword: string
}

export function RegisterPage() {
  const { t } = useTranslation()
  const navigate = useNavigate()
  const {
    register,
    handleSubmit,
    watch,
    formState: { errors },
  } = useForm<RegisterFormData>()

  const password = watch('password')

  const mutation = useMutation({
    mutationFn: registerCustomer,
    onSuccess: () => {
      navigate('/login', { state: { registered: true } })
    },
  })

  const onSubmit = (data: RegisterFormData) => {
    if (data.password !== data.confirmPassword) {
      return
    }
    mutation.mutate({ email: data.email, password: data.password })
  }

  let errorMessage = ''
  if (mutation.error) {
    const axiosError = mutation.error as AxiosError<{ detail?: string; code?: string }>
    if (axiosError.response?.data?.detail) {
      errorMessage = axiosError.response.data.detail
    } else {
      errorMessage = t('errors.unexpected')
    }
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
              {t('auth.register.title')}
            </Typography>

            <form onSubmit={handleSubmit(onSubmit)}>
              <Stack spacing={2}>
                {errorMessage && <Alert severity="error">{errorMessage}</Alert>}

                <TextField
                  {...register('email', {
                    required: true,
                    pattern: { value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/, message: '' },
                  })}
                  label={t('auth.register.email')}
                  type="email"
                  fullWidth
                  disabled={mutation.isPending}
                  error={!!errors.email}
                />

                <TextField
                  {...register('password', {
                    required: true,
                    minLength: { value: 8, message: '' },
                  })}
                  label={t('auth.register.password')}
                  type="password"
                  fullWidth
                  disabled={mutation.isPending}
                  error={!!errors.password}
                />

                <TextField
                  {...register('confirmPassword', {
                    required: true,
                    validate: (v) => v === password,
                  })}
                  label={t('auth.register.confirmPassword')}
                  type="password"
                  fullWidth
                  disabled={mutation.isPending}
                  error={!!errors.confirmPassword}
                />

                <Button
                  type="submit"
                  variant="contained"
                  fullWidth
                  disabled={mutation.isPending}
                  sx={{ position: 'relative' }}
                >
                  {mutation.isPending ? (
                    <CircularProgress
                      size={24}
                      sx={{ position: 'absolute', left: '50%', marginLeft: '-12px' }}
                    />
                  ) : null}
                  <span style={{ visibility: mutation.isPending ? 'hidden' : 'visible' }}>
                    {t('auth.register.submit')}
                  </span>
                </Button>

                <Typography align="center">
                  {t('auth.register.hasAccount')}{' '}
                  <Link href="/login" underline="hover">
                    {t('auth.register.loginLink')}
                  </Link>
                </Typography>
              </Stack>
            </form>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}
