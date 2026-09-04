import { useEffect, useState } from 'react'
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
  Chip,
} from '@mui/material'
import { getProfile, updateProfile, type UserProfile, type UpdateProfileRequest } from '../api/profile'

interface ProfileFormData {
  email: string
  phoneNumber: string
}

export function ProfileForm() {
  const { t } = useTranslation()
  const [profile, setProfile] = useState<UserProfile | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [isSaving, setIsSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [showSuccess, setShowSuccess] = useState(false)
  const [emailChanged, setEmailChanged] = useState(false)

  const { register, handleSubmit, formState: { errors }, reset } = useForm<ProfileFormData>({
    defaultValues: { email: '', phoneNumber: '' },
  })

  // Load profile on mount
  useEffect(() => {
    const loadProfile = async () => {
      try {
        setIsLoading(true)
        setError(null)
        const data = await getProfile()
        setProfile(data)
        reset({
          email: data.email || '',
          phoneNumber: data.phoneNumber || '',
        })
      } catch (err: any) {
        setError(t('auth.profile.errors.loadFailed'))
      } finally {
        setIsLoading(false)
      }
    }

    loadProfile()
  }, [reset, t])

  const originalEmail = profile?.email || ''

  const onSubmit = async (data: ProfileFormData) => {
    try {
      setIsSaving(true)
      setError(null)

      const updateData: UpdateProfileRequest = {}

      // Only include email if it changed
      if (data.email !== originalEmail) {
        updateData.email = data.email || null
        setEmailChanged(true)
      }

      // Only include phoneNumber if it's not empty
      if (data.phoneNumber) {
        updateData.phoneNumber = data.phoneNumber
      } else {
        updateData.phoneNumber = null
      }

      const updatedProfile = await updateProfile(updateData)
      setProfile(updatedProfile)
      reset({
        email: updatedProfile.email || '',
        phoneNumber: updatedProfile.phoneNumber || '',
      })
      setShowSuccess(true)
    } catch (err: any) {
      const errorMsg =
        err?.response?.data?.detail ||
        err?.response?.data?.message ||
        err?.message ||
        t('auth.profile.errors.updateFailed')
      setError(errorMsg)
    } finally {
      setIsSaving(false)
    }
  }

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box sx={{ maxWidth: 400 }}>
      <Typography variant="h6" component="h2" gutterBottom>
        {t('auth.profile.title')}
      </Typography>

      <form onSubmit={handleSubmit(onSubmit)}>
        <Stack spacing={2}>
          {error && (
            <Alert severity="error">
              {error}
            </Alert>
          )}

          {emailChanged && !error && (
            <Alert severity="info">
              {t('auth.profile.emailChangeWarning')}
            </Alert>
          )}

          {/* Display-only fields */}
          <Box>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              {t('auth.profile.username')}
            </Typography>
            <Typography variant="body2">
              {profile?.userName || t('auth.profile.notProvided')}
            </Typography>
          </Box>

          {/* Editable email field */}
          <TextField
            {...register('email', {
              validate: (value) => {
                if (!value) return true // Allow empty
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
                return emailRegex.test(value) || t('auth.profile.errors.invalidEmail')
              },
            })}
            label={t('auth.profile.email')}
            type="email"
            fullWidth
            disabled={isSaving}
            error={!!errors.email}
            helperText={errors.email?.message}
          />

          {/* Editable phone field */}
          <TextField
            {...register('phoneNumber', {
              validate: (value) => {
                if (!value) return true // Allow empty
                const phoneRegex = /^\+?[0-9\s\-()]{6,20}$/
                return phoneRegex.test(value) || t('auth.profile.errors.invalidPhone')
              },
            })}
            label={t('auth.profile.phone')}
            fullWidth
            disabled={isSaving}
            error={!!errors.phoneNumber}
            helperText={errors.phoneNumber?.message}
            placeholder="+1 (555) 123-4567"
          />

          {/* Email confirmation status */}
          <Box>
            <Typography variant="subtitle2" color="textSecondary" gutterBottom>
              {t('auth.profile.emailStatus')}
            </Typography>
            <Chip
              label={
                profile?.emailConfirmed
                  ? t('auth.profile.emailVerified')
                  : t('auth.profile.emailUnverified')
              }
              color={profile?.emailConfirmed ? 'success' : 'warning'}
              size="small"
              variant="outlined"
            />
          </Box>

          {/* Roles (display-only) */}
          {profile?.roles && profile.roles.length > 0 && (
            <Box>
              <Typography variant="subtitle2" color="textSecondary" gutterBottom>
                {t('auth.profile.roles')}
              </Typography>
              <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
                {profile.roles.map((role) => (
                  <Chip key={role} label={role} size="small" variant="outlined" />
                ))}
              </Stack>
            </Box>
          )}

          <Button
            type="submit"
            variant="contained"
            fullWidth
            disabled={isSaving}
            sx={{ position: 'relative' }}
          >
            {isSaving ? (
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
                  {t('auth.profile.saving')}
                </span>
              </>
            ) : (
              t('auth.profile.save')
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
          {t('auth.profile.success')}
        </Alert>
      </Snackbar>
    </Box>
  )
}
