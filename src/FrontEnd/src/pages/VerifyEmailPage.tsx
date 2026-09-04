import { useEffect, useState } from 'react'
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
} from '@mui/material'
import { postVerifyEmail, postResendVerification } from '@/features/auth/api/emailVerification'

type VerificationState = 'verifying' | 'success' | 'error' | 'resending'

interface ResendFormData {
  email: string
}

export function VerifyEmailPage() {
  const { t } = useTranslation()
  const [searchParams] = useSearchParams()

  const userId = searchParams.get('userId')
  const token = searchParams.get('token')
  // Derivable synchronously from the URL - not the async verification
  // outcome - so it seeds the initial state directly instead of being
  // pushed into state from inside the effect (avoids
  // react-hooks/set-state-in-effect: a lazy initializer run during render
  // is not "calling setState synchronously within an effect").
  const missingParams = !userId || !token

  const [state, setState] = useState<VerificationState>(missingParams ? 'error' : 'verifying')
  const [errorMessage, setErrorMessage] = useState(
    missingParams ? t('auth.verify.errors.invalidLink') : '',
  )

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ResendFormData>({
    defaultValues: { email: '' },
  })

  // Verify email on mount
  useEffect(() => {
    if (!userId || !token) {
      return
    }

    const verifyEmail = async () => {
      try {
        await postVerifyEmail({ userId, token })
        setState('success')
      } catch (error: any) {
        setState('error')
        setErrorMessage(
          error?.response?.data?.message ||
            error?.message ||
            t('auth.verify.errors.verificationFailed'),
        )
      }
    }

    verifyEmail()
  }, [userId, token, t])

  const onResendSubmit = async (data: ResendFormData) => {
    setState('resending')
    try {
      await postResendVerification({ email: data.email })
      setState('error')
      setErrorMessage(t('auth.verify.resendSuccess'))
    } catch (error: any) {
      setState('error')
      setErrorMessage(
        error?.response?.data?.message || error?.message || t('auth.verify.errors.resendFailed'),
      )
    }
  }

  if (state === 'verifying') {
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
            <Stack spacing={3} sx={{ alignItems: 'center' }}>
              <CircularProgress />
              <Typography variant="h6" align="center">
                {t('auth.verify.verifying')}
              </Typography>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    )
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
                <AlertTitle>{t('auth.verify.successTitle')}</AlertTitle>
                {t('auth.verify.successMessage')}
              </Alert>

              <Button variant="contained" fullWidth component={Link} to="/login">
                {t('auth.verify.backToLogin')}
              </Button>
            </Stack>
          </CardContent>
        </Card>
      </Box>
    )
  }

  // Error or resending state
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
              {t('auth.verify.title')}
            </Typography>

            {errorMessage && <Alert severity="error">{errorMessage}</Alert>}

            <Typography variant="body2" color="textSecondary" align="center">
              {t('auth.verify.resendInstructions')}
            </Typography>

            <form onSubmit={handleSubmit(onResendSubmit)}>
              <Stack spacing={2}>
                <TextField
                  {...register('email', {
                    required: true,
                    pattern: {
                      value: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                      message: t('auth.verify.errors.invalidEmail'),
                    },
                  })}
                  label={t('auth.verify.email')}
                  type="email"
                  fullWidth
                  disabled={state === 'resending'}
                  error={!!errors.email}
                  helperText={errors.email?.message}
                />

                <Button
                  type="submit"
                  variant="contained"
                  fullWidth
                  disabled={state === 'resending'}
                  sx={{ position: 'relative' }}
                >
                  {state === 'resending' ? (
                    <>
                      <CircularProgress
                        size={24}
                        sx={{
                          position: 'absolute',
                          left: '50%',
                          marginLeft: '-12px',
                        }}
                      />
                      <span style={{ visibility: 'hidden' }}>{t('auth.verify.resending')}</span>
                    </>
                  ) : (
                    t('auth.verify.resendButton')
                  )}
                </Button>
              </Stack>
            </form>

            <Button variant="text" fullWidth component={Link} to="/login">
              {t('auth.verify.backToLogin')}
            </Button>
          </Stack>
        </CardContent>
      </Card>
    </Box>
  )
}
