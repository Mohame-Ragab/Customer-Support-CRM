import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Container,
  Typography,
  Table,
  TableHead,
  TableBody,
  TableRow,
  TableCell,
  TextField,
  Switch,
  Button,
  Box,
  Snackbar,
  Alert,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { EmptyState } from '@/components/ui/EmptyState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import {
  useSystemSettings,
  useUpdateSystemSetting,
} from '@/features/security-admin/system-configuration/hooks/useSystemSettings'
import type { SystemSetting } from '@/features/security-admin/system-configuration/types'

function ValueEditor({
  setting,
  value,
  onChange,
  disabled,
}: {
  setting: SystemSetting
  value: string
  onChange: (value: string) => void
  disabled: boolean
}) {
  if (setting.valueType === 'Boolean') {
    return (
      <Switch
        checked={value.toLowerCase() === 'true'}
        onChange={(e) => onChange(e.target.checked ? 'true' : 'false')}
        disabled={disabled}
      />
    )
  }

  if (setting.valueType === 'Integer' || setting.valueType === 'Decimal') {
    return (
      <TextField
        type="number"
        size="small"
        value={value}
        onChange={(e) => onChange(e.target.value)}
        disabled={disabled}
      />
    )
  }

  return (
    <TextField
      multiline
      size="small"
      fullWidth
      value={value}
      onChange={(e) => onChange(e.target.value)}
      disabled={disabled}
    />
  )
}

export function SystemConfigurationPage() {
  const { t } = useTranslation()
  const { data, isLoading, isError, error, refetch } = useSystemSettings()
  const updateSetting = useUpdateSystemSetting()

  const [drafts, setDrafts] = useState<Record<string, string>>({})
  const [rowErrors, setRowErrors] = useState<Record<string, string>>({})
  const [savedKey, setSavedKey] = useState<string | null>(null)

  const valueFor = (setting: SystemSetting) => drafts[setting.key] ?? setting.value

  const handleSave = (setting: SystemSetting) => {
    const value = valueFor(setting)
    setRowErrors((prev) => ({ ...prev, [setting.key]: '' }))

    updateSetting.mutate(
      { key: setting.key, body: { value } },
      {
        onSuccess: () => {
          setSavedKey(setting.key)
          setDrafts((prev) => {
            const next = { ...prev }
            delete next[setting.key]
            return next
          })
        },
        onError: (err) => {
          const apiError = normalizeApiError(err)
          const message =
            apiError.validationErrors?.Value?.[0] ??
            apiError.detail ??
            t('systemConfiguration.errors.invalidValue')
          setRowErrors((prev) => ({ ...prev, [setting.key]: message }))
        },
      },
    )
  }

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        {t('systemConfiguration.title')}
      </Typography>
      <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
        {t('systemConfiguration.description', {
          defaultValue: 'Runtime operational settings. Changes take effect immediately.',
        })}
      </Typography>

      {isLoading && <LoadingState />}

      {isError && (
        <ErrorState
          title={t('errors.unexpected')}
          message={normalizeApiError(error).detail}
          onRetry={() => refetch()}
        />
      )}

      {!isLoading && !isError && data && data.length === 0 && (
        <EmptyState
          title={t('systemConfiguration.empty', { defaultValue: 'No settings found.' })}
        />
      )}

      {!isLoading && !isError && data && data.length > 0 && (
        <Box sx={{ overflowX: 'auto' }}>
          <Table>
            <TableHead>
              <TableRow>
                <TableCell>{t('systemConfiguration.columns.key')}</TableCell>
                <TableCell>{t('systemConfiguration.columns.type')}</TableCell>
                <TableCell>{t('systemConfiguration.columns.value')}</TableCell>
                <TableCell>{t('systemConfiguration.columns.description')}</TableCell>
                <TableCell>{t('systemConfiguration.columns.required')}</TableCell>
                <TableCell align="right">{t('systemConfiguration.columns.actions')}</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {data.map((setting) => {
                const value = valueFor(setting)
                const isDirty = value !== setting.value
                const rowError = rowErrors[setting.key]
                return (
                  <TableRow key={setting.key} hover>
                    <TableCell>{setting.key}</TableCell>
                    <TableCell>{setting.valueType}</TableCell>
                    <TableCell sx={{ minWidth: 220 }}>
                      <ValueEditor
                        setting={setting}
                        value={value}
                        onChange={(v) => setDrafts((prev) => ({ ...prev, [setting.key]: v }))}
                        disabled={updateSetting.isPending}
                      />
                      {rowError && (
                        <Typography variant="caption" color="error" sx={{ display: 'block' }}>
                          {rowError}
                        </Typography>
                      )}
                    </TableCell>
                    <TableCell>{setting.description ?? '—'}</TableCell>
                    <TableCell>
                      {setting.isRequired
                        ? t('systemConfiguration.yes', { defaultValue: 'Yes' })
                        : t('systemConfiguration.no', { defaultValue: 'No' })}
                    </TableCell>
                    <TableCell align="right">
                      <Button
                        size="small"
                        variant="contained"
                        disabled={!isDirty || updateSetting.isPending}
                        onClick={() => handleSave(setting)}
                      >
                        {t('systemConfiguration.save')}
                      </Button>
                    </TableCell>
                  </TableRow>
                )
              })}
            </TableBody>
          </Table>
        </Box>
      )}

      <Snackbar open={!!savedKey} autoHideDuration={4000} onClose={() => setSavedKey(null)}>
        <Alert severity="success" onClose={() => setSavedKey(null)}>
          {t('systemConfiguration.saved')}
        </Alert>
      </Snackbar>
    </Container>
  )
}
