import { useState } from 'react'
import { useTranslation } from 'react-i18next'
import {
  Alert,
  Box,
  Button,
  Container,
  Snackbar,
  Stack,
  TextField,
  Typography,
} from '@mui/material'
import { LoadingState } from '@/components/ui/LoadingState'
import { ErrorState } from '@/components/ui/ErrorState'
import { normalizeApiError } from '@/lib/api/normalizeApiError'
import { useQuickReplies } from '../hooks/useQuickReplies'
import { useQuickReplyMutations } from '../hooks/useQuickReplyMutations'
import type { QuickReplyTemplate } from '../types/quickReply'
import { QuickReplyList } from '../components/QuickReplyList'
import { QuickReplyForm } from '../components/QuickReplyForm'

export function QuickRepliesPage() {
  const { t } = useTranslation()
  const [search, setSearch] = useState('')
  const {
    data: templates,
    isPending,
    isError,
    error,
    refetch,
  } = useQuickReplies(search || undefined)
  const { createMutation, updateMutation, deleteMutation } = useQuickReplyMutations()

  const [dialogOpen, setDialogOpen] = useState(false)
  const [editing, setEditing] = useState<QuickReplyTemplate | null>(null)
  const [formError, setFormError] = useState<string | null>(null)
  const [snackbarMessage, setSnackbarMessage] = useState<string | null>(null)

  const openCreate = () => {
    setEditing(null)
    setFormError(null)
    setDialogOpen(true)
  }

  const openEdit = (template: QuickReplyTemplate) => {
    setEditing(template)
    setFormError(null)
    setDialogOpen(true)
  }

  const handleSubmit = (values: { name: string; body: string }) => {
    const mutation = editing
      ? updateMutation.mutateAsync({ id: editing.id, input: values })
      : createMutation.mutateAsync(values)

    mutation
      .then(() => setDialogOpen(false))
      .catch((err) => {
        const apiError = normalizeApiError(err)
        setFormError(
          apiError.validationErrors?.Name?.[0] ??
            apiError.validationErrors?.Body?.[0] ??
            (apiError.status === 409 ? t('quickReplies.errors.duplicateName') : apiError.detail) ??
            t('errors.unexpected'),
        )
      })
  }

  const handleDelete = (template: QuickReplyTemplate) => {
    if (!window.confirm(t('quickReplies.confirmDelete'))) return
    deleteMutation.mutate(template.id, {
      onError: (err) => setSnackbarMessage(normalizeApiError(err).detail ?? t('errors.unexpected')),
    })
  }

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Stack direction="row" sx={{ alignItems: 'center', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h4" component="h1">
          {t('quickReplies.title')}
        </Typography>
        <Button variant="contained" onClick={openCreate}>
          {t('quickReplies.create')}
        </Button>
      </Stack>

      <Box sx={{ mb: 2 }}>
        <TextField
          size="small"
          fullWidth
          label={t('quickReplies.search')}
          value={search}
          onChange={(e) => setSearch(e.target.value)}
        />
      </Box>

      {isPending && <LoadingState />}
      {isError && (
        <ErrorState message={normalizeApiError(error).detail} onRetry={() => refetch()} />
      )}
      {!isPending && !isError && (
        <QuickReplyList templates={templates ?? []} onEdit={openEdit} onDelete={handleDelete} />
      )}

      <QuickReplyForm
        open={dialogOpen}
        template={editing}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
        submitting={createMutation.isPending || updateMutation.isPending}
        error={formError}
      />

      <Snackbar
        open={!!snackbarMessage}
        autoHideDuration={5000}
        onClose={() => setSnackbarMessage(null)}
      >
        <Alert severity="error" onClose={() => setSnackbarMessage(null)}>
          {snackbarMessage}
        </Alert>
      </Snackbar>
    </Container>
  )
}
