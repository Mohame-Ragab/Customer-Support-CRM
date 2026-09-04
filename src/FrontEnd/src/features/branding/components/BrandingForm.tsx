import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Box,
  Button,
  TextField,
  Stack,
  Typography,
  Avatar,
  Alert,
  CircularProgress,
} from '@mui/material'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useBranding } from '../hooks/useBranding'
import { useUpdateBranding } from '../hooks/useUpdateBranding'

const HEX_COLOR_REGEX = /^#(?:[0-9a-fA-F]{3}|[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$/
const ALLOWED_LOGO_TYPES = ['image/png', 'image/jpeg', 'image/svg+xml', 'image/webp']
const MAX_LOGO_BYTES = 512 * 1024

interface BrandingFormProps {
  onSaved?: () => void
}

export function BrandingForm({ onSaved }: BrandingFormProps) {
  const { t } = useTranslation()
  const { data: current, isLoading } = useBranding()
  const updateBranding = useUpdateBranding()

  // Track only user edits here; the displayed value falls back to the loaded
  // branding (and finally a hardcoded default) so there is no need to sync
  // fetched data into local state via an effect - avoids a redundant render
  // pass and a setState-in-effect lint violation.
  const [primaryColorEdit, setPrimaryColorEdit] = useState<string | undefined>(undefined)
  const [secondaryColorEdit, setSecondaryColorEdit] = useState<string | undefined>(undefined)
  const [logoBase64, setLogoBase64] = useState<string | null>(null)
  const [logoContentType, setLogoContentType] = useState<string | null>(null)
  const [logoPreviewEdit, setLogoPreviewEdit] = useState<string | null | undefined>(undefined)
  const [error, setError] = useState<string | null>(null)

  const primaryColor = primaryColorEdit ?? current?.primaryColor ?? '#1565c0'
  const secondaryColor = secondaryColorEdit ?? current?.secondaryColor ?? '#546e7a'
  const logoPreview = logoPreviewEdit !== undefined ? logoPreviewEdit : (current?.logoDataUrl ?? null)

  const primaryValid = HEX_COLOR_REGEX.test(primaryColor)
  const secondaryValid = HEX_COLOR_REGEX.test(secondaryColor)

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setError(null)
    const file = e.target.files?.[0]
    if (!file) return

    if (!ALLOWED_LOGO_TYPES.includes(file.type)) {
      setError(t('branding.invalidLogoType', { defaultValue: 'Unsupported logo file type.' }))
      return
    }

    if (file.size > MAX_LOGO_BYTES) {
      setError(t('branding.logoTooLarge'))
      return
    }

    const reader = new FileReader()
    reader.onload = () => {
      const dataUrl = reader.result as string
      const base64 = dataUrl.split(',')[1] ?? ''
      setLogoBase64(base64)
      setLogoContentType(file.type)
      setLogoPreviewEdit(dataUrl)
    }
    reader.readAsDataURL(file)
  }

  const handleSave = () => {
    setError(null)
    updateBranding.mutate(
      {
        primaryColor,
        secondaryColor,
        logoBase64,
        logoContentType,
      },
      {
        onSuccess: () => {
          setLogoBase64(null)
          setLogoContentType(null)
          onSaved?.()
        },
        onError: (err) => {
          setError(normalizeApiError(err).detail ?? t('branding.errors.saveFailed', { defaultValue: 'Failed to save branding.' }))
        },
      },
    )
  }

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
        <CircularProgress />
      </Box>
    )
  }

  return (
    <Box sx={{ maxWidth: 480 }}>
      <Stack spacing={3}>
        {error && <Alert severity="error">{error}</Alert>}

        <TextField
          label={t('branding.primaryColor')}
          type="text"
          value={primaryColor}
          onChange={(e) => setPrimaryColorEdit(e.target.value)}
          error={!primaryValid}
          helperText={!primaryValid ? t('branding.invalidColor') : undefined}
          slotProps={{
            input: {
              startAdornment: (
                <Box
                  sx={{
                    width: 24,
                    height: 24,
                    borderRadius: 1,
                    bgcolor: primaryValid ? primaryColor : 'transparent',
                    border: '1px solid',
                    borderColor: 'divider',
                    mr: 1,
                  }}
                />
              ),
            },
          }}
        />

        <TextField
          label={t('branding.secondaryColor')}
          type="text"
          value={secondaryColor}
          onChange={(e) => setSecondaryColorEdit(e.target.value)}
          error={!secondaryValid}
          helperText={!secondaryValid ? t('branding.invalidColor') : undefined}
          slotProps={{
            input: {
              startAdornment: (
                <Box
                  sx={{
                    width: 24,
                    height: 24,
                    borderRadius: 1,
                    bgcolor: secondaryValid ? secondaryColor : 'transparent',
                    border: '1px solid',
                    borderColor: 'divider',
                    mr: 1,
                  }}
                />
              ),
            },
          }}
        />

        <Box>
          <Typography variant="subtitle2" gutterBottom>
            {t('branding.logo')}
          </Typography>
          <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
            <Avatar src={logoPreview ?? undefined} variant="rounded" sx={{ width: 56, height: 56 }} />
            <Button variant="outlined" component="label">
              {t('branding.upload')}
              <input
                type="file"
                accept="image/png,image/jpeg,image/svg+xml,image/webp"
                hidden
                onChange={handleFileChange}
              />
            </Button>
          </Stack>
        </Box>

        <Button
          variant="contained"
          disabled={!primaryValid || !secondaryValid || updateBranding.isPending}
          onClick={handleSave}
        >
          {updateBranding.isPending ? <CircularProgress size={20} /> : t('branding.save')}
        </Button>
      </Stack>
    </Box>
  )
}
