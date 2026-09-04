import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import { Box, MenuItem, TextField, Button, Stack, Alert, Snackbar } from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useTicketCategoriesQuery } from '../hooks/useTicketCategoriesQuery'
import { useSetTicketClassificationMutation } from '../hooks/useSetTicketClassificationMutation'
import { TicketPriority } from '../types'

interface TicketClassificationEditorProps {
  ticketId: string
  initialCategoryId: string | null
  initialPriority: TicketPriority | null
}

const PRIORITIES: TicketPriority[] = [
  TicketPriority.Low,
  TicketPriority.Medium,
  TicketPriority.High,
  TicketPriority.Urgent,
]

const PRIORITY_LABEL_KEYS: Record<TicketPriority, string> = {
  [TicketPriority.Low]: 'tickets.classification.priorityLevels.low',
  [TicketPriority.Medium]: 'tickets.classification.priorityLevels.medium',
  [TicketPriority.High]: 'tickets.classification.priorityLevels.high',
  [TicketPriority.Urgent]: 'tickets.classification.priorityLevels.urgent',
}

export function TicketClassificationEditor({
  ticketId,
  initialCategoryId,
  initialPriority,
}: TicketClassificationEditorProps) {
  const { t, i18n } = useTranslation()
  const { data: categories, isLoading: categoriesLoading } = useTicketCategoriesQuery()
  const mutation = useSetTicketClassificationMutation(ticketId)

  const [categoryId, setCategoryId] = useState(initialCategoryId ?? '')
  const [priority, setPriority] = useState<TicketPriority | ''>(initialPriority ?? '')
  const [error, setError] = useState<string | null>(null)
  const [showSuccess, setShowSuccess] = useState(false)

  const isDirty = categoryId !== (initialCategoryId ?? '') || priority !== (initialPriority ?? '')

  const handleSave = () => {
    setError(null)
    mutation.mutate(
      {
        categoryId: categoryId || null,
        priority: priority === '' ? null : priority,
      },
      {
        onSuccess: () => setShowSuccess(true),
        onError: (err) => {
          const apiError = normalizeApiError(err)
          setError(
            apiError.validationErrors?.CategoryId?.[0] ??
              apiError.detail ??
              t('tickets.classification.saveFailed', {
                defaultValue: 'Failed to save classification.',
              }),
          )
        },
      },
    )
  }

  if (categoriesLoading) {
    return <LoadingState />
  }

  return (
    <Box sx={{ maxWidth: 400 }}>
      <Stack spacing={2}>
        {error && <Alert severity="error">{error}</Alert>}

        <TextField
          select
          label={t('tickets.classification.categoryLabel')}
          value={categoryId}
          onChange={(e) => setCategoryId(e.target.value)}
          disabled={mutation.isPending}
        >
          <MenuItem value="">—</MenuItem>
          {categories?.map((category) => (
            <MenuItem key={category.id} value={category.id}>
              {i18n.language === 'ar' ? category.nameAr : category.nameEn}
            </MenuItem>
          ))}
        </TextField>

        <TextField
          select
          label={t('tickets.classification.priorityLabel')}
          value={priority}
          onChange={(e) =>
            setPriority(e.target.value === '' ? '' : (Number(e.target.value) as TicketPriority))
          }
          disabled={mutation.isPending}
        >
          <MenuItem value="">—</MenuItem>
          {PRIORITIES.map((p) => (
            <MenuItem key={p} value={p}>
              {t(PRIORITY_LABEL_KEYS[p])}
            </MenuItem>
          ))}
        </TextField>

        <Button variant="contained" disabled={!isDirty || mutation.isPending} onClick={handleSave}>
          {t('tickets.classification.save')}
        </Button>
      </Stack>

      <Snackbar open={showSuccess} autoHideDuration={4000} onClose={() => setShowSuccess(false)}>
        <Alert severity="success" onClose={() => setShowSuccess(false)}>
          {t('tickets.classification.saved')}
        </Alert>
      </Snackbar>
    </Box>
  )
}
